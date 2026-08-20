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

        // DDL Slip code generator
        services.AddScoped<Himapp.Execution.Contracts.References.IDdlSlipCodeGenerator, Himapp.Execution.Application.Features.DailyDepartmentalLabourSlip.Services.DdlSlipCodeGenerator>();
        // DLR code generator (for DailyLabor DLR-(ProjectCode)-0001 style codes)
        services.AddScoped<Himapp.Execution.Contracts.References.IDlrCodeGenerator, Himapp.Execution.Application.Features.DailyLabor.Services.DlrCodeGenerator>();

        // Public schema project lookup (reads ProjectMaster from public schema using DB connection)
        services.AddScoped<Himapp.Execution.Contracts.References.IReferenceLookupService, Himapp.Execution.Application.Lookups.PublicSchemaReferenceLookup>();

        return services;
    }
}
