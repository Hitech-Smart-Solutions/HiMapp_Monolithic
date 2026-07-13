using System.Collections.Concurrent;
using Himapp.SharedKernel.Abstractions;
using Himapp.Workflow.Models;

namespace Himapp.Workflow.Services;

public sealed class InMemoryWorkflowService : IWorkflowService
{
    private readonly ConcurrentDictionary<long, WorkflowInstance> _instances = [];
    private readonly IClock _clock;
    private long _nextId;

    public InMemoryWorkflowService(IClock clock) => _clock = clock;

    public Task<WorkflowInstance> StartAsync(string workflowType, string entityName, long entityId, long actorUserId, CancellationToken cancellationToken = default)
    {
        var id = Interlocked.Increment(ref _nextId);
        var instance = new WorkflowInstance
        {
            WorkflowType = workflowType,
            EntityName = entityName,
            EntityId = entityId
        };

        instance.MoveTo("Submitted", new ApprovalHistory(id, 0, "Start", actorUserId, null, _clock.UtcNow));
        _instances[id] = instance;
        return Task.FromResult(instance);
    }

    public Task<WorkflowInstance> FireAsync(long workflowId, string trigger, long actorUserId, string? comment, CancellationToken cancellationToken = default)
    {
        if (!_instances.TryGetValue(workflowId, out var instance))
        {
            throw new InvalidOperationException($"Workflow {workflowId} was not found.");
        }

        var nextState = trigger switch
        {
            "ApproveL1" => "L1Approved",
            "ApproveL2" => "Approved",
            "Reject" => "Rejected",
            "Cancel" => "Cancelled",
            "Dispute" => "Disputed",
            "Resolve" => "Resolved",
            _ => trigger
        };

        instance.MoveTo(nextState, new ApprovalHistory(workflowId, instance.History.Count + 1, trigger, actorUserId, comment, _clock.UtcNow));
        return Task.FromResult(instance);
    }
}
