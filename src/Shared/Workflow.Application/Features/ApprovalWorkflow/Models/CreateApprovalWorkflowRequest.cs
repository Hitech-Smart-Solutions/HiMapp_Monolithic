namespace Himapp.Workflow.Application.Features.ApprovalWorkflow.Models;

public sealed record CreateApprovalWorkflowRequest(
    string ApprovalWorkflowCode,
    DateTime? ApprovalWorkflowDate,
    int ProgramId,
    int? CompanyId,
    int? LocationId,
    int? StatusId,
    int CreatedBy,
    List<ApprovalWorkflowProjectDetailRequest>? ProjectDetails = null,
    List<ApprovalWorkflowRoleDetailRequest>? RoleDetails = null);
