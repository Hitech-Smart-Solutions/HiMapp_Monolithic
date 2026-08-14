using MediatR;
using Himapp.Execution.Application.Features.DailyProgress.Models;

namespace Himapp.Execution.Application.Features.DailyProgress.Queries;

public sealed record GetAllDailyProgressQuery : IRequest<IReadOnlyCollection<DailyProgressModel>>;
public sealed record GetDailyProgressByIdQuery(int Id) : IRequest<DailyProgressModel?>;
