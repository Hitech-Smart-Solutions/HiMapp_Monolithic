namespace Himapp.Workflow.Application.Features.Workflow.Models;

public sealed record ApprovalHistory(
    int WorkflowId,
    int Level,
    string Action,
    int ActorUserId,
    string? Comment,
    DateTimeOffset ActionAt);
