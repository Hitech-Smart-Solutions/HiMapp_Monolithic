using Himapp.Files.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Himapp.Files;

public static class DependencyInjection
{
    public static IServiceCollection AddHimappFiles(this IServiceCollection services)
    {
        services.AddSingleton<IFileService, InMemoryFileService>();
        return services;
    }
}
