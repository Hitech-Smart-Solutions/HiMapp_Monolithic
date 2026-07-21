using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Himapp.Store.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddStoreInfrastructure(this IServiceCollection services, Action<DbContextOptionsBuilder> configureDb)
    {
        services.AddDbContext<StoreDbContext>(configureDb);
        return services;
    }
}

