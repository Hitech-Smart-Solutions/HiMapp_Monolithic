using Himapp.Store.Application.Lookups;
using Himapp.Store.Application.Features.GatePasses;
using Himapp.Store.Application.Infrastructure;
using Himapp.Store.Contracts.GatePass;
using Microsoft.Extensions.DependencyInjection;

namespace Himapp.Store.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddStoreModule(this IServiceCollection services)
    {
        services.AddMediatR(configuration =>
            configuration.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

        services.AddSingleton<IGatePassRepository, InMemoryGatePassRepository>();
        services.AddSingleton<IBackgroundTaskQueue, InMemoryBackgroundTaskQueue>();
        services.AddHostedService<QueuedBackgroundService>();
        services.AddSingleton<IGatePassLookup, InMemoryGatePassLookup>();
        return services;
    }
}
