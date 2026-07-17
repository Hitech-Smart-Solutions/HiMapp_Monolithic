using MediatR;
using Himapp.Execution.Application.Features.DailyLabor.Models;

namespace Himapp.Execution.Application.Features.DailyLabor.Commands;

public sealed record CreateDailyLaborCommand(CreateDailyLaborRequest Request) : IRequest<DailyLaborModel>;
public sealed record UpdateDailyLaborCommand(long Id, UpdateDailyLaborRequest Request) : IRequest<DailyLaborModel?>;
public sealed record DeleteDailyLaborCommand(long Id) : IRequest<bool>;
