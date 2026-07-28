using Himapp.Audit.Abstractions;
using Himapp.Audit.Models;
using Himapp.SharedKernel.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Himapp.Audit.Filters;

/// <summary>
/// Global action filter that automatically logs user actions (Create/Update/Delete/View)
/// to the TransactionActionHistory table.
/// 
/// This filter is registered as a global filter in <see cref="DependencyInjection"/>
/// and applies to ALL controllers dynamically.
/// 
/// How it works:
/// 1. On successful responses (200-299), it extracts action metadata
/// 2. Maps HTTP method to ActionId (POST→Inserted[501], PUT→Updated[502], DELETE→Deleted[503], GET→Viewed[506])
/// 3. Extracts ProgramId from response DTO if it implements <see cref="IHasProgramId"/>
/// 4. Extracts entity ID from response (e.g., .Id property)
/// 5. Gets entity name from the route pattern (e.g., "v1/execution/daily-progress" → "DailyProgress")
/// 6. Queues the log entry via <see cref="IAuditService"/> asynchronously
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public sealed class AutoUserActionLogAttribute : Attribute, IAsyncActionFilter
{
    /// <summary>
    /// Mapping from HTTP method to ActionId (from <see cref="Actions"/> enum).
    /// </summary>
    private static readonly Dictionary<string, int> HttpMethodToActionId = new(StringComparer.OrdinalIgnoreCase)
    {
        ["POST"]   = (int)Actions.Inserted,
        ["PUT"]    = (int)Actions.Updated,
        ["PATCH"]  = (int)Actions.Updated,
        ["DELETE"] = (int)Actions.Deleted,
        ["GET"]    = (int)Actions.Viewed
    };

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var httpMethod = context.HttpContext.Request.Method;

        // Determine action ID from HTTP method (default to Viewed)
        if (!HttpMethodToActionId.TryGetValue(httpMethod, out var actionId))
        {
            await next();
            return;
        }

        // Continue with the action execution
        var executed = await next();

        // Only log on successful responses (200-299)
        if (executed.Result is ObjectResult { StatusCode: >= 200 and <= 299 } objectResult)
        {
            await LogIfApplicable(context, objectResult, httpMethod, actionId);
        }
        // Also handle status code results (e.g., 200 OK without body for deletes)
        else if (executed.Result is StatusCodeResult { StatusCode: >= 200 and <= 299 } statusCodeResult)
        {
            await LogIfApplicable(context, null, httpMethod, actionId);
        }
    }

    private async Task LogIfApplicable(
        ActionExecutingContext context,
        ObjectResult? objectResult,
        string httpMethod,
        int actionId)
    {
        try
        {
            var serviceProvider = context.HttpContext.RequestServices;

            // Resolve dependencies
            var auditService = serviceProvider.GetRequiredService<IAuditService>();
            var currentUser = serviceProvider.GetRequiredService<ICurrentUser>();
            var logger = serviceProvider.GetRequiredService<ILogger<AutoUserActionLogAttribute>>();

            // Extract UserId from authenticated session
            var userId = (int)(currentUser.UserId ?? 0);
            if (userId == 0)
            {
                // Anonymous user — skip logging
                return;
            }

            // Extract ProgramId from the response body (if it implements IHasProgramId)
            int programId = 0;
            long programRowId = 0;
            string? programRowCode = null;

            if (objectResult?.Value is IHasProgramId hasProgramId)
            {
                programId = (int)hasProgramId.ProgramId;
            }
            else if (objectResult?.Value != null)
            {
                // Try to extract via reflection for common DTO patterns
                var valueType = objectResult.Value.GetType();
                TryExtractProperty(valueType, objectResult.Value, "ProgramId", out programId);
                TryExtractProperty(valueType, objectResult.Value, "ProjectId", out programId);
            }

            // Extract entity/record ID from response body
            if (objectResult?.Value != null)
            {
                var valueType = objectResult.Value.GetType();
                TryExtractProperty(valueType, objectResult.Value, "Id", out programRowId);
            }

            // Extract entity/record ID from route (e.g., api/workflow/DailyLabor/123)
            if (programRowId == 0 && context.RouteData.Values.TryGetValue("id", out var routeId))
            {
                long.TryParse(routeId?.ToString(), out programRowId);
            }

            // Derive entity name from route pattern
            // e.g., "v1/execution/daily-progress" → "DailyProgress"
            programRowCode = DeriveEntityName(context);

            // Determine channel from request header (default to "Web")
            var channel = context.HttpContext.Request.Headers["X-Client-Type"].FirstOrDefault() ?? "Web";

            // Build remarks with channel info
            var remarks = $"Action via {channel}";

            // Queue the log entry asynchronously (non-blocking)
            await auditService.LogAsync(
                userId,
                actionId,
                programId,
                (int)programRowId,
                programRowCode,
                remarks,
                context.HttpContext.RequestAborted);
        }
        catch (Exception ex)
        {
            // Logging should never break the main flow
            var logger = context.HttpContext.RequestServices
                .GetRequiredService<ILogger<AutoUserActionLogAttribute>>();
            logger.LogWarning(ex, "Failed to queue audit log entry. Action will continue unaffected.");
        }
    }

    /// <summary>
    /// Extracts the entity name from the route pattern.
    /// e.g., "v1/execution/daily-progress" → "DailyProgress"
    /// e.g., "v1/admin/labours" → "Labour"
    /// e.g., "api/workflow/DailyLabor" → "Workflow"
    /// </summary>
    private static string? DeriveEntityName(ActionExecutingContext context)
    {
        // Try to get the last meaningful segment from the route
        var path = context.HttpContext.Request.Path.Value ?? string.Empty;

        // Remove leading numbers and version prefixes
        var segments = path.Split('/')
            .Where(s => !string.IsNullOrEmpty(s) && !s.StartsWith("v") && !char.IsDigit(s[0]))
            .ToArray();

        if (segments.Length > 0)
        {
            // Take the last relevant segment as the entity name
            var lastSegment = segments.Last();
            // Convert kebab-case to PascalCase
            return string.Join("", lastSegment.Split('-', '_')
                .Select(s => char.ToUpper(s[0]) + s[1..]));
        }

        // Fallback: use controller name from route data
        if (context.RouteData.Values.TryGetValue("controller", out var controller))
        {
            return controller?.ToString();
        }

        return null;
    }

    /// <summary>
    /// Tries to extract a property value by name from an object using reflection.
    /// </summary>
    private static void TryExtractProperty(Type type, object obj, string propertyName, out int result)
    {
        result = 0;
        try
        {
            var prop = type.GetProperty(propertyName);
            if (prop != null && prop.CanRead)
            {
                var value = prop.GetValue(obj);
                if (value != null)
                {
                    result = Convert.ToInt32(value);
                }
            }
        }
        catch
        {
            // Ignore reflection errors
        }
    }

    /// <summary>
    /// Tries to extract a long property value by name from an object using reflection.
    /// </summary>
    private static void TryExtractProperty(Type type, object obj, string propertyName, out long result)
    {
        result = 0;
        try
        {
            var prop = type.GetProperty(propertyName);
            if (prop != null && prop.CanRead)
            {
                var value = prop.GetValue(obj);
                if (value != null)
                {
                    result = Convert.ToInt64(value);
                }
            }
        }
        catch
        {
            // Ignore reflection errors
        }
    }
}

