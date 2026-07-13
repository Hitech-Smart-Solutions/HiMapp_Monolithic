using Himapp.Admin.Contracts.Contractors;
using Himapp.Admin.Contracts.Labour;
using Himapp.Admin.Contracts.Projects;
using Himapp.Admin.Contracts.WorkCategories;
using Himapp.Admin.Application.Features.Labours;
using Himapp.Admin.Application.Lookups;
using Microsoft.Extensions.DependencyInjection;

namespace Himapp.Admin.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddAdminModule(this IServiceCollection services)
    {
        services.AddMediatR(configuration =>
            configuration.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

        services.AddSingleton<ILabourRepository, InMemoryLabourRepository>();
        services.AddSingleton<ILabourLookup, InMemoryAdminLookup>();
        services.AddSingleton<IContractorLookup, InMemoryAdminLookup>();
        services.AddSingleton<IProjectDirectory, InMemoryAdminLookup>();
        services.AddSingleton<IWorkCategoryLookup, InMemoryAdminLookup>();
        return services;
    }
}
