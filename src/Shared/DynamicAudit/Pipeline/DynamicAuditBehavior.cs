using System.Diagnostics;
using System.Reflection;
using System.Text.Json;
using Himapp.DynamicAudit.Abstractions;
using Himapp.DynamicAudit.Models;
using Himapp.SharedKernel.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Himapp.DynamicAudit.Pipeline;

/// <summary>
/// MediatR pipeline behavior that logs an audit entry for any request
/// decorated with <see cref="AuditLogAttribute"/>.
/// Requests without the attribute are passed through with zero overhead
/// (the check is a static field resolved once per closed generic type).
/// </summary>
public sealed class DynamicAuditBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    // Resolved once per TRequest type — no per-call reflection cost.
    private static readonly bool _shouldAudit =
        typeof(TRequest).GetCustomAttribute<AuditLogAttribute>() is not null;

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = false,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private readonly IDynamicAuditService _auditService;
    private readonly ICurrentUser _currentUser;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public DynamicAuditBehavior(
        IDynamicAuditService auditService,
        ICurrentUser currentUser,
        IHttpContextAccessor httpContextAccessor)
    {
        _auditService = auditService;
        _currentUser = currentUser;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        // Opt-in: skip immediately if the attribute is absent.
        if (!_shouldAudit)
            return await next();

        var http     = _httpContextAccessor.HttpContext;
        var traceId  = http?.TraceIdentifier ?? Guid.NewGuid().ToString("N")[..8].ToUpper();
        var userId   = _currentUser.UserId ?? 0;
        var userName = http?.User?.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value
                    ?? http?.User?.FindFirst("name")?.Value
                    ?? "anonymous";
        var payload  = TrySerialize(request);

        var sw = Stopwatch.StartNew();

        try
        {
            var response = await next();
            sw.Stop();

            await _auditService.LogAsync(new DynamicAuditLog
            {
                TraceId        = traceId,
                UserId         = userId,
                UserName       = userName,
                RequestName    = typeof(TRequest).Name,
                RequestPayload = payload,
                Status         = "SUCCESS",
                DurationMs     = sw.ElapsedMilliseconds,
                Timestamp      = DateTime.UtcNow,
            }, cancellationToken);

            return response;
        }
        catch (Exception ex)
        {
            sw.Stop();

            await _auditService.LogAsync(new DynamicAuditLog
            {
                TraceId        = traceId,
                UserId         = userId,
                UserName       = userName,
                RequestName    = typeof(TRequest).Name,
                RequestPayload = payload,
                Status         = "FAILED",
                ErrorMessage   = ex.Message,
                DurationMs     = sw.ElapsedMilliseconds,
                Timestamp      = DateTime.UtcNow,
            }, cancellationToken);

            throw;
        }
    }

    private static string? TrySerialize(TRequest request)
    {
        try { return JsonSerializer.Serialize(request, _jsonOptions); }
        catch { return null; }
    }
}
