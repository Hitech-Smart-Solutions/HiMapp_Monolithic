using Himapp.Execution.Application.Lookups;
using Himapp.Execution.Application.Features.Activities;
using Himapp.Execution.Contracts.Dpr;
using Microsoft.Extensions.DependencyInjection;

namespace Himapp.Execution.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddExecutionModule(this IServiceCollection services)
    {
        services.AddMediatR(configuration =>
            configuration.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

        // prefer scoped EF repositories for production usage
        services.AddScoped<IActivityRepository, EfActivityRepository>();
        services.AddScoped<Features.ProjectActivities.IProjectActivityRepository, Features.ProjectActivities.EfProjectActivityRepository>();
        services.AddScoped<Features.RateMaster.IRateMasterRepository, Features.RateMaster.EfRateMasterRepository>();
        // Repositories for other execution features
        services.AddScoped<Features.Area.IAreaRepository, Features.Area.EfAreaRepository>();
        services.AddScoped<Features.Uom.IUomRepository, Features.Uom.EfUomRepository>();
        services.AddScoped<Features.Manpower.IManpowerRepository, Features.Manpower.EfManpowerRepository>();
        services.AddScoped<Features.DailyProgress.IDailyProgressRepository, Features.DailyProgress.EfDailyProgressRepository>();
        services.AddScoped<Features.DailyLabor.IDailyLaborRepository, Features.DailyLabor.EfDailyLaborRepository>();
        services.AddScoped<Features.Planning.IPlanningRepository, Features.Planning.EfPlanningRepository>();
        services.AddSingleton<IDprLookup, InMemoryDprLookup>();
        return services;
    }
}
