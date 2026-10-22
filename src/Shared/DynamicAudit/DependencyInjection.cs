using Himapp.DynamicAudit.Abstractions;
using Himapp.DynamicAudit.Pipeline;
using Himapp.DynamicAudit.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Himapp.DynamicAudit;

public static class DependencyInjection
{
    /// <summary>
    /// Registers the DynamicAudit module:
    /// - <see cref="DynamicAuditDbContext"/> (dedicated EF context)
    /// - <see cref="IDynamicAuditService"/> (channel-based async log queue)
    /// - <see cref="BackgroundDynamicAuditConsumer"/> (background batch writer)
    /// - <see cref="DynamicAuditBehavior{TRequest,TResponse}"/> (MediatR pipeline, opt-in via [AuditLog])
    /// </summary>
    public static IServiceCollection AddDynamicAudit(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? configuration.GetConnectionString("Default");

        services.AddDbContext<DynamicAuditDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddSingleton<DynamicAuditService>();
        services.AddSingleton<IDynamicAuditService>(sp =>
            sp.GetRequiredService<DynamicAuditService>());

        services.AddHostedService<BackgroundDynamicAuditConsumer>();

        // Register as open-generic so every MediatR request goes through it,
        // but the behavior itself skips instantly when [AuditLog] is absent.
        services.AddTransient(
            typeof(IPipelineBehavior<,>),
            typeof(DynamicAuditBehavior<,>));

        return services;
    }
}
