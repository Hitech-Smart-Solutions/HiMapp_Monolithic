using MediatR;
using Himapp.Execution.Application.Features.DailyLabor.Models;

namespace Himapp.Execution.Application.Features.DailyLabor.Queries;

public sealed record GetDailyLaborByIdQuery(int Id) : IRequest<DailyLaborModel?>;
