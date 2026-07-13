using Himapp.SharedKernel;
using Himapp.Workflow.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Himapp.Workflow;

public static class DependencyInjection
{
    public static IServiceCollection AddHimappWorkflow(this IServiceCollection services)
    {
        services.AddSharedKernel();
        services.AddSingleton<IWorkflowService, InMemoryWorkflowService>();
        return services;
    }
}
