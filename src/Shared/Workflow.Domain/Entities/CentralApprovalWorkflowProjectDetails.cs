using System;
using System.Collections.Generic;
using System.Text;

namespace Himapp.Workflow.Domain.Entities;

public class CentralApprovalWorkflowProjectDetails
{
    public Guid UniqueID { get; set; }

    public int ID { get; set; }

    public int ApprovalWorkflowID { get; set; }

    public int ProjectID { get; set; }

    public int? StatusID { get; set; }

    public bool IsActive { get; set; } = true;

    public int CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public int LastModifiedBy { get; set; }

    public DateTime LastModifiedDate { get; set; }

    // Navigation Property
    public virtual CentralApprovalWorkflow? ApprovalWorkflow { get; set; }
}
