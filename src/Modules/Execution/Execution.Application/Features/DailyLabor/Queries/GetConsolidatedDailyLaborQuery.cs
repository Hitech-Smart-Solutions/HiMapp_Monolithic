using Himapp.Execution.Application.Features.DailyLabor.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Himapp.Execution.Application.Features.DailyLabor.Queries
{
    public sealed record GetConsolidatedDailyLaborQuery(int ProjectId, DateOnly Date) : IRequest<IReadOnlyCollection<DailyLaborConsolidatedModel>>;
}
