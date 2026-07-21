using Himapp.SharedKernel.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

// Outbox & Logging helpers
using Himapp.SharedKernel.Outbox;

namespace Himapp.SharedKernel;

public static class DependencyInjection
{
    public static IServiceCollection AddSharedKernel(this IServiceCollection services)
    {
        services.AddSingleton<IClock, SystemClock>();
        services.AddScoped<ICurrentUser, AnonymousCurrentUser>();
        // Outbox - stores outbound messages reliably and dispatches them in background
        services.AddScoped<IOutboxService, OutboxService>();
        services.AddHostedService<OutboxDispatcherHostedService>();
        return services;
    }
}
