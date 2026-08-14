using Himapp.SharedKernel.Abstractions;

namespace Himapp.Workflow.Domain.Entities;

public class CentralApprovalWorkflowRoleDetails
{
    public Guid UniqueID { get; set; }

    public int ID { get; set; }

    public int ApprovalWorkflowID { get; set; }

    public int RoleID { get; set; }

    public int? Priority { get; set; }

    public decimal? Amount { get; set; }

    public string? Remarks { get; set; }

    public bool? CanAuthorize { get; set; }

    public bool? CanUnAuthorize { get; set; }

    public int? StatusID { get; set; }

    public bool IsActive { get; set; } = true;

    public int CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public int LastModifiedBy { get; set; }

    public DateTime LastModifiedDate { get; set; }

    // Navigation Property
    public virtual CentralApprovalWorkflow? ApprovalWorkflow { get; set; }
}
