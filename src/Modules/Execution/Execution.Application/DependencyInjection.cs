using Himapp.Execution.Application.Features.DailyDepartmentalLabourSlip.Services;
using Himapp.Execution.Application.Features.DailyProgress.Service;
using Himapp.Execution.Application.Features.Planning.Services;
using Himapp.Execution.Application.Features.Planning.Services.IServices;
using Himapp.Execution.Application.Lookups;
using Himapp.Execution.Contracts.Dpr;
using Himapp.Execution.Contracts.References;
using Microsoft.Extensions.DependencyInjection;
using Himapp.Execution.Application.Features.DailyLabor.Services;

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
        services.AddScoped<IDPRCodeGenerator, DPRCodeGenerator>();

        // DDL Slip code generator
        services.AddScoped<IDdlSlipCodeGenerator, DdlSlipCodeGenerator>();
        // DLR code generator (for DailyLabor DLR-(ProjectCode)-0001 style codes)
        services.AddScoped<IDlrCodeGenerator, DlrCodeGenerator>();

        // Public schema project lookup (reads ProjectMaster from public schema using DB connection)
        services.AddScoped<IReferenceLookupService, PublicSchemaReferenceLookup>();

        return services;
    }
}
