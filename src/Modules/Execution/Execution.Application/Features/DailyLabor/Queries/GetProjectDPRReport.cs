using MediatR;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Himapp.Execution.Application.Features.DailyLabor.Queries
{
    public sealed record GetProjectDPRReport(
   int Type,
   DateTime? FromDate,
   DateTime? ToDate,
   int Project,
   int Activity,
   int Contractor,
   int Section, 
   bool? Departmental, 
   string? SortColumn = "ReportDate desc", 
   int PageIndex = 0, 
   int PageSize = 10
) : IRequest<DataSet>;
}
