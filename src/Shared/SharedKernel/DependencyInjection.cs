using Himapp.SharedKernel.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Himapp.SharedKernel;

public static class DependencyInjection
{
    public static IServiceCollection AddSharedKernel(this IServiceCollection services)
    {
        services.AddSingleton<IClock, SystemClock>();
        services.AddScoped<ICurrentUser, AnonymousCurrentUser>();
        return services;
    }
}
