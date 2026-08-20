using Microsoft.Extensions.DependencyInjection;

namespace Himapp.Workflow.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddWorkflowModule(this IServiceCollection services)
    {
        services.AddMediatR(configuration =>
            configuration.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));
        return services;
    }
}
