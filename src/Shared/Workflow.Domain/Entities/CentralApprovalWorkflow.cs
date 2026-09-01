using Himapp.SharedKernel.Abstractions;

namespace Himapp.Workflow.Domain.Entities;

public class CentralApprovalWorkflow : BaseEntity
{
    public CentralApprovalWorkflow()
    {
        ApprovalWorkflowProjectDetails = new HashSet<CentralApprovalWorkflowProjectDetails>();
        ApprovalWorkflowRoleDetails = new HashSet<CentralApprovalWorkflowRoleDetails>();
    }

    public string ApprovalWorkflowCode { get; set; } = string.Empty;

    public string ApprovalWorkflowName { get; set; }

    public DateTime? ApprovalWorkflowDate { get; set; }

    public int ProgramID { get; set; }

    public int? CompanyID { get; set; }

    public int LocationID { get; set; }

    public int? WorkflowTypeID { get; set; }

    public int? StatusID { get; set; }

    public bool IsActive { get; set; } = true;

    // Navigation Property
    public virtual ICollection<CentralApprovalWorkflowProjectDetails>? ApprovalWorkflowProjectDetails { get; set; }
    public virtual ICollection<CentralApprovalWorkflowRoleDetails>? ApprovalWorkflowRoleDetails { get; set; }
}
