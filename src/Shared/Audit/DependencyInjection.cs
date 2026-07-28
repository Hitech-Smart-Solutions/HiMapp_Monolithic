using Himapp.Audit.Abstractions;
using Himapp.Audit.Filters;
using Himapp.Audit.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Himapp.Audit;

/// <summary>
/// Extension methods for registering audit logging services.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registers audit logging services:
    /// - <see cref="IAuditService"/> (channel-based async logging)
    /// - <see cref="AuditService"/> (singleton, shared channel state)
    /// - <see cref="BackgroundAuditConsumer"/> (background batch writer)
    /// - <see cref="AutoUserActionLogAttribute"/> as a global MVC filter
    /// </summary>
    public static IServiceCollection AddAuditLogging(this IServiceCollection services)
    {
        // Register the audit service as both singleton (for channel sharing) and interface
        services.AddSingleton<AuditService>();
        services.AddSingleton<IAuditService>(sp => sp.GetRequiredService<AuditService>());

        // Background consumer to batch-write logs to DB
        services.AddHostedService<BackgroundAuditConsumer>();

        // Register the auto-log action filter as a global filter
        // This applies to ALL controllers automatically
        services.AddMvc(options =>
        {
            options.Filters.Add<AutoUserActionLogAttribute>();
        });

        return services;
    }
}

