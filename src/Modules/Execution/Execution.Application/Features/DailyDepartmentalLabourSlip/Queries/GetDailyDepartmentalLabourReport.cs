using MediatR;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Himapp.Execution.Application.Features.DailyDepartmentalLabourSlip.Queries
{
    public sealed record GetDailyDepartmentalLabourReport(
     int Project = 0,
     DateTime? FromDate = null,
     DateTime? ToDate = null,
     int Contractor = 0,
     int Activity = 0,
     int Location = 0,
     bool? IsLumpSum = null,
     string? SortColumn = "SlipDate desc",
     int PageIndex = 0,
     int PageSize = 10
 ) : IRequest<DataSet>;
}
