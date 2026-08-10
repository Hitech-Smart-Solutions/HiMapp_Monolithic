using System;
using System.Collections.Generic;
using System.Text;

namespace Himapp.Execution.Application.Features.DailyLabor.Models
{
    public sealed record DailyLaborConsolidatedModel(int? ContractorID,int? ActivityID,int Skilled,int Unskilled,int Mat,int Total);
}
