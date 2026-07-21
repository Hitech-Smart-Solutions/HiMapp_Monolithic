using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Himapp.PM.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPMInfrastructure(this IServiceCollection services, Action<DbContextOptionsBuilder> configureDb)
    {
        services.AddDbContext<PMDbContext>(configureDb);
        return services;
    }
}

