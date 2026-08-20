namespace Himapp.Workflow.Application.Features.ApprovalWorkflow.Models;

public sealed record ApprovalWorkflowRoleDetailDto(
    int Id,
    Guid UniqueId,
    int RoleId,
    int? Priority,
    decimal? Amount,
    string? Remarks,
    bool? CanAuthorize,
    bool? CanUnAuthorize,
    int? StatusId,
    bool IsActive,
    int CreatedBy,
    DateTime CreatedDate,
    int LastModifiedBy,
    DateTime LastModifiedDate);
