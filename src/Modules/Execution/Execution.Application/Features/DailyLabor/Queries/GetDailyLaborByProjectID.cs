using MediatR;
using System;
using System.Collections.Generic;
using Himapp.Execution.Application.Features.DailyLabor.Models;
using System.Text;

namespace Himapp.Execution.Application.Features.DailyLabor.Queries
{
    public sealed record GetDailyLaborByProjectID(SearchParamsProjectWise SearchParamsProjectWise) : IRequest<PagedResult<DailyLaborModel>>;
}
