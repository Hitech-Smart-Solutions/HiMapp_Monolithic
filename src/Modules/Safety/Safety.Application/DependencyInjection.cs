using Himapp.Safety.Application.Lookups;
using Himapp.Safety.Application.Features.Incidents;
using Himapp.Safety.Contracts.Clearance;
using Microsoft.Extensions.DependencyInjection;

namespace Himapp.Safety.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddSafetyModule(this IServiceCollection services)
    {
        services.AddMediatR(configuration =>
            configuration.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

        services.AddSingleton<IIncidentRepository, InMemoryIncidentRepository>();
        services.AddSingleton<ILabourClearanceLookup, InMemoryLabourClearanceLookup>();
        return services;
    }
}
