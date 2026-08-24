using Himapp.Execution.Application.Features.DailyLabor.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Himapp.Execution.Application.Features.DailyLabor.Queries;

public sealed record DPRGetConsolidatedDailyLaborQuery(DateOnly Date, int ProjectId) : IRequest<IReadOnlyCollection<DPRDailyLaborConsolidatedModel>>;
