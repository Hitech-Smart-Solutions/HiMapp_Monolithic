using Himapp.SharedKernel.Abstractions;

namespace Himapp.Workflow.Domain.Entities;

public class CentralApprovalWorkflowRoleDetails : BaseEntity
{

    public int ApprovalWorkflowID { get; set; }

    public int RoleID { get; set; }

    public int? Priority { get; set; }

    public decimal? Amount { get; set; }

    public string? Remarks { get; set; }

    public bool? CanAuthorize { get; set; }

    public bool? CanUnAuthorize { get; set; }

    public int? StatusID { get; set; }

    public bool IsActive { get; set; } = true;

    // Navigation Property
    public virtual CentralApprovalWorkflow? ApprovalWorkflow { get; set; }
}
