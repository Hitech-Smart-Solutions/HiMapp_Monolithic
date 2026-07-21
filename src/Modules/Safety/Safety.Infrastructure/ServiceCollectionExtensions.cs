using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Himapp.Safety.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSafetyInfrastructure(this IServiceCollection services, Action<DbContextOptionsBuilder> configureDb)
    {
        services.AddDbContext<SafetyDbContext>(configureDb);
        return services;
    }
}

