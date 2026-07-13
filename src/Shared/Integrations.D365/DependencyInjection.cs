using Himapp.Integrations.D365.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Himapp.Integrations.D365;

public static class DependencyInjection
{
    public static IServiceCollection AddD365Integration(this IServiceCollection services)
    {
        services.AddSingleton<ID365SyncService, NoopD365SyncService>();
        return services;
    }
}
