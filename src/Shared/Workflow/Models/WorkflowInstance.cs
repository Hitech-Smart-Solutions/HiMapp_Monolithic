using Himapp.SharedKernel.Abstractions;

namespace Himapp.Workflow.Models;

public sealed class WorkflowInstance : BaseEntity
{
    public string WorkflowType { get; init; } = string.Empty;
    public string EntityName { get; init; } = string.Empty;
    public long EntityId { get; init; }
    public string CurrentState { get; private set; } = "Draft";
    public List<ApprovalHistory> History { get; } = [];

    public void MoveTo(string state, ApprovalHistory history)
    {
        CurrentState = state;
        History.Add(history);
    }
}
