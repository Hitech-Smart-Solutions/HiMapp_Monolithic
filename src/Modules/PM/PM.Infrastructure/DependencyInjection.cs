using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Himapp.PM.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddPMInfrastructure<TContext>(this IServiceCollection services, IConfiguration configuration)
        where TContext : DbContext
    {
        // Register the application's DbContext type provided by the host
        services.AddDbContext<TContext>(options =>
        {
            var conn = configuration.GetConnectionString("DefaultConnection");
            options.UseNpgsql(conn);
        });

        return services;
    }
}

