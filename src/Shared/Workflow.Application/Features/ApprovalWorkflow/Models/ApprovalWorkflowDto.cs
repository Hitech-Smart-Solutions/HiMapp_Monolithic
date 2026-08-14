namespace Himapp.Workflow.Application.Features.ApprovalWorkflow.Models;

public sealed record ApprovalWorkflowDto(
    int Id,
    Guid UniqueId,
    string ApprovalWorkflowCode,
    DateTime? ApprovalWorkflowDate,
    int ProgramId,
    int? CompanyId,
    int? LocationId,
    int? StatusId,
    bool IsActive,
    int CreatedBy,
    DateTime CreatedDate,
    int LastModifiedBy,
    DateTime LastModifiedDate,
    IReadOnlyCollection<ApprovalWorkflowProjectDetailDto> ProjectDetails,
    IReadOnlyCollection<ApprovalWorkflowRoleDetailDto> RoleDetails);
