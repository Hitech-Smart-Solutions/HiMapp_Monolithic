using Himapp.Audit.Abstractions;
using Himapp.Audit.Filters;
using Himapp.Audit.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Himapp.Audit;

/// <summary>
/// Extension methods for registering audit logging services.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registers audit logging services:
    /// - <see cref="AuditDbContext"/> (dedicated EF context for TransactionActionHistory)
    /// - <see cref="IAuditService"/> (channel-based async logging)
    /// - <see cref="AuditService"/> (singleton, shared channel state)
    /// - <see cref="BackgroundAuditConsumer"/> (background batch writer)
    /// </summary>
    public static IServiceCollection AddAuditLogging(this IServiceCollection services, IConfiguration configuration)
    {
        // Register dedicated AuditDbContext using the same PostgreSQL connection
        var connectionString = configuration.GetConnectionString("Default")
            ?? configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<AuditDbContext>(options =>
            options.UseNpgsql(connectionString));

        // Register the audit service as both singleton (for channel sharing) and interface
        services.AddSingleton<AuditService>();
        services.AddSingleton<IAuditService>(sp => sp.GetRequiredService<AuditService>());

        // Background consumer to batch-write logs to DB
        services.AddHostedService<BackgroundAuditConsumer>();

        return services;
    }

    /// <summary>
    /// Registers the global auto-log action filter on the MVC controller options.
    /// Must be called AFTER AddControllers() in Program.cs.
    /// </summary>
    public static IMvcBuilder AddAuditActionFilter(this IMvcBuilder mvcBuilder)
    {
        mvcBuilder.AddMvcOptions(options =>
        {
            options.Filters.Add<AutoUserActionLogAttribute>();
        });
        return mvcBuilder;
    }
}
