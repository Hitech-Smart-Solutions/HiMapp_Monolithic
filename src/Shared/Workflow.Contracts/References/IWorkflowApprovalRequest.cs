using System;
using System.Collections.Generic;
using System.Text;

namespace Himapp.Workflow.Contracts.References
{
    public interface IWorkflowApprovalRequest
    {
        int ProjectId { get; }
        int StatusId { get; }
        string? Remarks { get; }
    }
}
