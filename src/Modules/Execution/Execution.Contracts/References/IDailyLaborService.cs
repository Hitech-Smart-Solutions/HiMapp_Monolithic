using System;
using System.Collections.Generic;
using System.Text;
using Himapp.Execution.Contracts.DailyLabor;

namespace Himapp.Execution.Contracts.References;

public interface IDailyLaborService
{
    Task<IReadOnlyCollection<DPRDailyLaborConsolidatedModel>>
        GetDPRConsolidatedDailyLaborAsync(
            int projectId,
            DateOnly date,
            CancellationToken cancellationToken = default);
}
