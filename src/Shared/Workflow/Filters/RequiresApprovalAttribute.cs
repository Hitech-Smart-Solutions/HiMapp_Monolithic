using Himapp.SharedKernel.Abstractions;
using Himapp.Workflow.Models;
using Himapp.Workflow.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace Himapp.Workflow.Filters;

/// <summary>
/// ASP.NET Core action filter attribute.
/// Decorate a controller class or action method to auto-create a workflow
/// instance after a successful POST (201 Created) response.
/// 
/// The response DTO must implement <see cref="IRequiresApproval"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public sealed class RequiresApprovalAttribute : Attribute, IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var executed = await next();

        // Only auto-create on 200/201 success responses
        if (executed.Result is ObjectResult { StatusCode: >= 200 and <= 299 } objectResult
            && objectResult.Value is IRequiresApproval approvalDto)
        {
            var workflowService = context.HttpContext.RequestServices.GetRequiredService<IWorkflowService>();
            var clock = context.HttpContext.RequestServices.GetRequiredService<IClock>();
            var currentUser = context.HttpContext.RequestServices.GetRequiredService<ICurrentUser>();

            // Mark the entity as "Submitted" (pending approval)
            await workflowService.StartAsync(
                approvalDto.EntityName,
                approvalDto.EntityName,
                approvalDto.EntityId,
                currentUser.UserId ?? 0,
                context.HttpContext.RequestAborted);
        }
    }
}

