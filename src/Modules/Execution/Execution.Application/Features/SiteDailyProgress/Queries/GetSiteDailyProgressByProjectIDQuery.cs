using MediatR;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Himapp.Execution.Application.Features.SiteDailyProgress.Queries
{
    public sealed record GetSiteDailyProgressByProjectIDQuery(SearchParamsProjectWise SearchParamsProjectWise) : IRequest<DataSet>;
}
