namespace Himapp.Workflow.Models;

public sealed record ApprovalHistory(
    long WorkflowId,
    int Level,
    string Action,
    long ActorUserId,
    string? Comment,
    DateTimeOffset ActionAt);
