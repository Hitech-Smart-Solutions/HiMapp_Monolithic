using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Himapp.Admin.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAdminInfrastructure(this IServiceCollection services, Action<DbContextOptionsBuilder> configureDb)
    {
        services.AddDbContext<AdminDbContext>(configureDb);
        return services;
    }
}

