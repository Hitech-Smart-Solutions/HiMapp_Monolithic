using Himapp.PM.Application.Lookups;
using Himapp.PM.Application.Features.Equipments;
using Himapp.PM.Contracts.Equipment;
using Microsoft.Extensions.DependencyInjection;
using Himapp.PM.Application.BackgroundJobs;
using Himapp.PM.Application.Services;
using Himapp.PM.Application.Interfaces;

namespace Himapp.PM.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddPlantMachineryModule(this IServiceCollection services)
    {
        services.AddMediatR(configuration =>
            configuration.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

        services.AddSingleton<IEquipmentRepository, InMemoryEquipmentRepository>();
        services.AddSingleton<IEquipmentLookup, InMemoryEquipmentLookup>();
        services.AddScoped<DynamicsAssetTypeJobs>();
        services.AddScoped<IDynamicsSyncLogService, DynamicsSyncLogService>();
        services.AddScoped<IEquipmentCategoryLinkService, EquipmentCategoryLinkService>();
        services.AddScoped<IEquipmentSetupService, EquipmentSetupService>();
        services.AddHttpClient();
        services.AddHttpClient<IDynamicsIntegrationService, DynamicsIntegrationService>();
        return services;
    }
}
