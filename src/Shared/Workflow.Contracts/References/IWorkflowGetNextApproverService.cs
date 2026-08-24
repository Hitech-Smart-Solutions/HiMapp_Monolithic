using System;
using System.Collections.Generic;
using System.Text;
using Himapp.Workflow.Contracts.Models;

namespace Himapp.Workflow.Contracts.References;

public interface IWorkflowGetNextApproverService
{
    Task<NextApproverModel?> GetNextApproverAsync(
        int projectId,
        int programId,
        int userId,
        int priority,
        CancellationToken cancellationToken);
}
