using MediatR;
using Himapp.Execution.Application.Features.DailyLabor.Models;

namespace Himapp.Execution.Application.Features.DailyLabor.Queries;

public sealed record GetAllDailyLaborsQuery : IRequest<IReadOnlyCollection<DailyLaborModel>>;
public sealed record GetDailyLaborByIdQuery(long Id) : IRequest<DailyLaborModel?>;
