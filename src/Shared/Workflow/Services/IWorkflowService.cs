using Himapp.Workflow.Models;

namespace Himapp.Workflow.Services;

public interface IWorkflowService
{
    Task<WorkflowInstance> StartAsync(string workflowType, string entityName, long entityId, long actorUserId, CancellationToken cancellationToken = default);

    Task<WorkflowInstance> FireAsync(long workflowId, string trigger, long actorUserId, string? comment, CancellationToken cancellationToken = default);
}
