using MediatR;
using Himapp.Execution.Application.Features.DailyLabor.Models;
using System.Collections.Generic;

namespace Himapp.Execution.Application.Features.DailyLabor.Queries;

public sealed record GetAllDailyLaborsQuery() : IRequest<IReadOnlyCollection<DailyLaborModel>>;
