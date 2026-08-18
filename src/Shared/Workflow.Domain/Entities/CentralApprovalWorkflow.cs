namespace Himapp.Workflow.Domain.Entities;

public class CentralApprovalWorkflow
{
    public CentralApprovalWorkflow()
    {
        ApprovalWorkflowProjectDetails = new HashSet<CentralApprovalWorkflowProjectDetails>();
        ApprovalWorkflowRoleDetails = new HashSet<CentralApprovalWorkflowRoleDetails>();
    }

    public Guid UniqueID { get; set; }

    public int ID { get; set; }

    public string ApprovalWorkflowCode { get; set; } = string.Empty;

    public string ApprovalWorkflowName { get; set; }

    public DateTime? ApprovalWorkflowDate { get; set; }

    public int ProgramID { get; set; }

    public int? CompanyID { get; set; }

    public int? LocationID { get; set; }

    public int? StatusID { get; set; }

    public bool IsActive { get; set; } = true;

    public int CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public int LastModifiedBy { get; set; }

    public DateTime LastModifiedDate { get; set; }

    // Navigation Property
    public virtual ICollection<CentralApprovalWorkflowProjectDetails>? ApprovalWorkflowProjectDetails { get; set; }
    public virtual ICollection<CentralApprovalWorkflowRoleDetails>? ApprovalWorkflowRoleDetails { get; set; }
}
