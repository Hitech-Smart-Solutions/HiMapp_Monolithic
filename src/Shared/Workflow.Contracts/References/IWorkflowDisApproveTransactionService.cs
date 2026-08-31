using System;
using System.Collections.Generic;
using System.Text;

namespace Himapp.Workflow.Contracts.References;

public interface IWorkflowDisApproveTransactionService
{
    Task DisApproveTransactionAsync(
        int id,
        int programId,
        string disApprovalRemarks,
        int actionedBy,
        int remarksId,
        CancellationToken cancellationToken);
}
