using Himapp.Execution.Application.Lookups;
using Himapp.Execution.Contracts.Dpr;
using Microsoft.Extensions.DependencyInjection;

namespace Himapp.Execution.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddExecutionModule(this IServiceCollection services)
    {
        services.AddMediatR(configuration =>
            configuration.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

        services.AddSingleton<IDprLookup, InMemoryDprLookup>();
        return services;
    }
}
