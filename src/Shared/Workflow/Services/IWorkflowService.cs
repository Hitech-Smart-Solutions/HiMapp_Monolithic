using Himapp.Workflow.Models;

namespace Himapp.Workflow.Services;

public interface IWorkflowService
{
    /// <summary>
    /// Start a new workflow for an entity + optional submit to level 1 approval.
    /// </summary>
    Task<WorkflowInstance> StartAsync(string workflowType, string entityName, int entityId, int actorUserId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Fire a trigger (approve, reject, etc.) against an existing workflow by its ID.
    /// </summary>
    Task<WorkflowInstance> FireAsync(int workflowId, string trigger, int actorUserId, string? comment, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get a workflow instance by its ID.
    /// </summary>
    Task<WorkflowInstance?> GetByIdAsync(int workflowId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Look up a workflow instance by entity name + entity ID.
    /// </summary>
    Task<WorkflowInstance?> GetByEntityAsync(string entityName, int entityId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all workflow instances currently in "PendingApproval" state.
    /// </summary>
    Task<IReadOnlyCollection<WorkflowInstance>> GetPendingAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get the workflow configuration for a given entity type.
    /// </summary>
    WorkflowConfiguration GetConfiguration(string entityName);
}
