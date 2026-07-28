using System.Collections.Concurrent;
using Himapp.SharedKernel.Abstractions;
using Himapp.Workflow.Models;

namespace Himapp.Workflow.Services;

public sealed class InMemoryWorkflowService : IWorkflowService
{
    private readonly ConcurrentDictionary<int, WorkflowInstance> _instances = [];
    private readonly IClock _clock;
    private int _nextId;

    public InMemoryWorkflowService(IClock clock) => _clock = clock;

    public Task<WorkflowInstance> StartAsync(string workflowType, string entityName, int entityId, int actorUserId, CancellationToken cancellationToken = default)
    {
        var id = Interlocked.Increment(ref _nextId);
        var instance = new WorkflowInstance
        {
            WorkflowType = workflowType,
            EntityName = entityName,
            EntityId = entityId
        };

        var config = GetConfiguration(entityName);
        var submittedState = config.SubmittedState;

        instance.MoveTo(submittedState, new ApprovalHistory(id, 0, WorkflowActions.Submit, actorUserId, null, _clock.UtcNow));
        _instances[id] = instance;
        return Task.FromResult(instance);
    }

    public Task<WorkflowInstance> FireAsync(int workflowId, string trigger, int actorUserId, string? comment, CancellationToken cancellationToken = default)
    {
        if (!_instances.TryGetValue(workflowId, out var instance))
        {
            throw new InvalidOperationException($"Workflow {workflowId} was not found.");
        }

        var config = GetConfiguration(instance.EntityName);

        var nextState = trigger switch
        {
            WorkflowActions.Approve => ResolveNextApprovalState(instance, config),
            WorkflowActions.Reject => config.RejectedState,
            WorkflowActions.Cancel => WorkflowStates.Cancelled,
            WorkflowActions.Dispute => WorkflowStates.Disputed,
            WorkflowActions.Resolve => WorkflowStates.Resolved,
            _ => trigger
        };

        var level = instance.History.Count + 1;
        instance.MoveTo(nextState, new ApprovalHistory(workflowId, level, trigger, actorUserId, comment, _clock.UtcNow));
        return Task.FromResult(instance);
    }

    public Task<WorkflowInstance?> GetByIdAsync(int workflowId, CancellationToken cancellationToken = default)
    {
        _instances.TryGetValue(workflowId, out var instance);
        return Task.FromResult(instance);
    }

    public Task<WorkflowInstance?> GetByEntityAsync(string entityName, int entityId, CancellationToken cancellationToken = default)
    {
        var instance = _instances.Values.FirstOrDefault(x =>
            x.EntityName.Equals(entityName, StringComparison.OrdinalIgnoreCase) && x.EntityId == entityId);
        return Task.FromResult(instance);
    }

    public Task<IReadOnlyCollection<WorkflowInstance>> GetPendingAsync(CancellationToken cancellationToken = default)
    {
        var pending = _instances.Values
            .Where(x => x.CurrentState == WorkflowStates.PendingApproval)
            .ToArray() as IReadOnlyCollection<WorkflowInstance>;
        return Task.FromResult(pending ?? Array.Empty<WorkflowInstance>());
    }

    public WorkflowConfiguration GetConfiguration(string entityName)
        => DefaultWorkflowConfigurations.GetFor(entityName);

    private static string ResolveNextApprovalState(WorkflowInstance instance, WorkflowConfiguration config)
    {
        // Determine which level the current approval is at
        var approvedCount = instance.History.Count(h => h.Action == WorkflowActions.Approve);
        var totalLevels = config.Levels.Count;

        if (approvedCount >= totalLevels)
            return config.ApprovedState;

        return config.SubmittedState; // still pending next level
    }
}
