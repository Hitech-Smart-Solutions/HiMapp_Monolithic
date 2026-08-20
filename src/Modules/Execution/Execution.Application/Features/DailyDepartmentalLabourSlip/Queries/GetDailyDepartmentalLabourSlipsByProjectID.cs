using MediatR;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Himapp.Execution.Application.Features.DailyDepartmentalLabourSlip.Queries
{
    public sealed record GetDailyDepartmentalLabourSlipsByProjectID(SearchParamsProjectWise SearchParamsProjectWise) : IRequest<DataSet>;
}
