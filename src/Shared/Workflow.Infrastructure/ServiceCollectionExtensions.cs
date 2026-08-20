using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Himapp.Workflow.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddWorkflowInfrastructure(this IServiceCollection services, Action<DbContextOptionsBuilder> configureDb)
    {
        services.AddDbContext<WorkflowDbContext>(configureDb);
        return services;
    }
}
