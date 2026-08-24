using Himapp.Workflow.Contracts.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Himapp.Workflow.Contracts.References;

public interface IWorkflowChangeApprovalService
{
    Task<ChangeApprovalModel?> ChangeApprovalAsync(
        int id,
        int projectId,
        int entityId,
        short statusId,
        string? remarks,
        int actionedBy,
        int nextApproverId,
        int priority,
        CancellationToken cancellationToken);
}
