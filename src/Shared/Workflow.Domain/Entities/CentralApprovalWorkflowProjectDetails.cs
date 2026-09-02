using Himapp.SharedKernel.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Himapp.Workflow.Domain.Entities;

public class CentralApprovalWorkflowProjectDetails : BaseEntity
{

    public int ApprovalWorkflowID { get; set; }

    public int ProjectID { get; set; }

    public int? StatusID { get; set; }

    public bool IsActive { get; set; } = true;

    // Navigation Property
    public virtual CentralApprovalWorkflow? ApprovalWorkflow { get; set; }
}
