using System;
using System.Collections.Generic;
using System.Text;

namespace Himapp.Execution.Contracts.DailyLabor;

public class DPRDailyLaborConsolidatedResponse
{
    public bool Status { get; set; }
    public string Message { get; set; } = string.Empty;
    public bool IsManpowerNotAvailable { get; set; }
    public IReadOnlyCollection<DPRDailyLaborConsolidatedModel> Data { get; set; }
        = Array.Empty<DPRDailyLaborConsolidatedModel>();
}
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
