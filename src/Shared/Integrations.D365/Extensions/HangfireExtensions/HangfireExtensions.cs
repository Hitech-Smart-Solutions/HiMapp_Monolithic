using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Integrations.D365.BackgroundJobs;
using Shared.Integrations.D365.BackgroundJobs.Interfaces;
using Shared.Integrations.D365.BackgroundJobs.Services;

namespace Shared.Integrations.D365.Extensions.HangfireExtensions;

public static class HangfireExtensions
{
    public static IServiceCollection AddSharedBackgroundJobs(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString =
            configuration.GetConnectionString("Default")
            ?? configuration.GetConnectionString("DefaultConnection");

        services.AddHangfire(config =>
        {
            config.UseSimpleAssemblyNameTypeSerializer()
                  .UseRecommendedSerializerSettings()
                  .UsePostgreSqlStorage(
                      options =>
                      {
                          options.UseNpgsqlConnection(
                              connectionString);
                      });
        });

        services.AddHangfireServer();

        services.AddScoped<
            IBackgroundJobService,
            HangfireBackgroundJobService>();

        return services;
    }
}