namespace Himapp.Workflow.Application.Features.ApprovalWorkflow.Models;

public sealed record ApprovalWorkflowRoleDetailRequest(
    int RoleId,
    int? Priority,
    decimal? Amount,
    string? Remarks,
    bool? CanAuthorize,
    bool? CanUnAuthorize,
    int? StatusId,
    bool IsActive = true);
