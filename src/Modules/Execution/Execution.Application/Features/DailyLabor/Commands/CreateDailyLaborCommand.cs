using MediatR;
using Himapp.Execution.Application.Features.DailyLabor.Models;

namespace Himapp.Execution.Application.Features.DailyLabor.Commands;

public sealed record CreateDailyLaborCommand(CreateDailyLaborRequest Request) : IRequest<DailyLaborModel>;
