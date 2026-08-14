namespace Himapp.Workflow.Application.Features.ApprovalWorkflow.Models;

public sealed record ApprovalWorkflowProjectDetailDto(
    int Id,
    Guid UniqueId,
    int ProjectId,
    int? StatusId,
    bool IsActive,
    int CreatedBy,
    DateTime CreatedDate,
    int LastModifiedBy,
    DateTime LastModifiedDate);
