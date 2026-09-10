using System;
using System.Collections.Generic;
using System.Text;
using Himapp.Workflow.Contracts.Models;

namespace Himapp.Workflow.Contracts.References;

public interface IWorkflowPendingApprovalsService
{
    Task<IReadOnlyList<AwaitingDailyProgressModel?>> GetAwaitingDailyProgress(int userId,CancellationToken cancellationToken);
    Task<IReadOnlyList<AwaitingDepartmentalLabourSlipModel?>> GetAwaitingDepartmentalLabourSlip(int userId, CancellationToken cancellationToken);
}
