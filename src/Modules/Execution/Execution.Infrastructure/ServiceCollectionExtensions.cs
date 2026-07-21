using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Himapp.Execution.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddExecutionInfrastructure(this IServiceCollection services, Action<DbContextOptionsBuilder> configureDb)
    {
        services.AddDbContext<ExecutionDbContext>(configureDb);
        return services;
    }
}
