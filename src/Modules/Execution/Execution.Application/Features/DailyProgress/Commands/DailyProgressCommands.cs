using MediatR;
using Himapp.Execution.Application.Features.DailyProgress.Models;

namespace Himapp.Execution.Application.Features.DailyProgress.Commands;

public sealed record CreateDailyProgressCommand(CreateDailyProgressRequest Request) : IRequest<DailyProgressModel>;
public sealed record UpdateDailyProgressCommand(int Id, UpdateDailyProgressRequest Request) : IRequest<DailyProgressModel?>;
public sealed record DeleteDailyProgressCommand(AddTransactionActionHistoryDTO dtoInactive) : IRequest<bool>;
