using Himapp.PM.Application.Lookups;
using Himapp.PM.Application.Features.Equipments;
using Himapp.PM.Contracts.Equipment;
using Microsoft.Extensions.DependencyInjection;

namespace Himapp.PM.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddPlantMachineryModule(this IServiceCollection services)
    {
        services.AddMediatR(configuration =>
            configuration.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

        services.AddSingleton<IEquipmentRepository, InMemoryEquipmentRepository>();
        services.AddSingleton<IEquipmentLookup, InMemoryEquipmentLookup>();
        return services;
    }
}
