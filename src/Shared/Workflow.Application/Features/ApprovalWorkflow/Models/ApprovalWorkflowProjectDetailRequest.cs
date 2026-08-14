namespace Himapp.Workflow.Application.Features.ApprovalWorkflow.Models;

public sealed record ApprovalWorkflowProjectDetailRequest(
    int ProjectId,
    int? StatusId,
    bool IsActive = true);
