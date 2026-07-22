using MediatR;
using Himapp.Execution.Application.Features.DailyLabor.Models;

namespace Himapp.Execution.Application.Features.DailyLabor.Commands;

public sealed record UpdateDailyLaborCommand(int Id, UpdateDailyLaborRequest Request) : IRequest<DailyLaborModel?>;
