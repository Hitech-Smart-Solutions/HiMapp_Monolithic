using Himapp.Workflow.Application;
using Himapp.Workflow.Application.Features.Workflow.Services;
using Himapp.Workflow.Contracts.References;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Himapp.Workflow.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddWorkflowInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<WorkflowDbContext>(options =>
        {
            var conn = configuration.GetConnectionString("DefaultConnection");
            options.UseNpgsql(conn);
        });

        services.AddScoped<IWorkflowDbContext>(sp => sp.GetRequiredService<WorkflowDbContext>());


        services.AddScoped<IWorkflowGetNextApproverService,WorkflowGetNextApproverService>();

        services.AddScoped<IWorkflowChangeApprovalService,WorkflowChangeApprovalService>();

        services.AddScoped<IWorkflowPendingApprovalsService, WorkflowPendingApprovalsService>();

        services.AddScoped<IWorkflowDisApproveTransactionService, WorkflowDisApproveTransactionService>();

        return services;
    }
}
