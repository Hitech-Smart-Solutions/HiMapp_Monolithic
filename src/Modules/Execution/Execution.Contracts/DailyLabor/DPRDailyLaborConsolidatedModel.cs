using System;
using System.Collections.Generic;
using System.Text;

namespace Himapp.Execution.Contracts.DailyLabor;

public sealed record DPRDailyLaborConsolidatedModel(
    int? ContractorID,
    string? ContractorName,
    int? ActivityID,
    string? ActivityName,
    int Skilled,
    int Unskilled,
    int Mat,
    int Total
);
