using Himapp.Execution.Application.Lookups;
using Himapp.Execution.Contracts.Dpr;
using Himapp.Execution.Application.Features.Planning.Services;
using Microsoft.Extensions.DependencyInjection;
using Himapp.Execution.Application.Features.Planning.Services.IServices;

namespace Himapp.Execution.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddExecutionModule(this IServiceCollection services)
    {
        services.AddMediatR(configuration =>
            configuration.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

        services.AddSingleton<IDprLookup, InMemoryDprLookup>();

        // Planning: section lookup and Excel importer
        services.AddScoped<IPlanningSectionService, PlanningSectionService>();
        services.AddScoped<IExcelPlanningImporter, ExcelPlanningImporter>();

        return services;
    }
}
