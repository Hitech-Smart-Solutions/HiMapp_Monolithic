using MediatR;
using Himapp.Execution.Application.Features.DailyProgress.Models;

namespace Himapp.Execution.Application.Features.DailyProgress.Commands;

public sealed record CreateDailyProgressCommand(CreateDailyProgressRequest Request) : IRequest<DailyProgressModel>;
public sealed record UpdateDailyProgressCommand(long Id, UpdateDailyProgressRequest Request) : IRequest<DailyProgressModel?>;
public sealed record DeleteDailyProgressCommand(long Id) : IRequest<bool>;
