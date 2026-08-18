namespace Himapp.Workflow.Application.Features.ApprovalWorkflow.Models;

public sealed record UpdateApprovalWorkflowRequest(
    string ApprovalWorkflowCode,
    string ApprovalWorkflowName,
    DateTime? ApprovalWorkflowDate,
    int ProgramId,
    int? CompanyId,
    int LocationId,
    int? WorkflowTypeId,
    int? StatusId,
    bool IsActive,
    int LastModifiedBy,
    List<ApprovalWorkflowProjectDetailRequest>? ProjectDetails = null,
    List<ApprovalWorkflowRoleDetailRequest>? RoleDetails = null);
