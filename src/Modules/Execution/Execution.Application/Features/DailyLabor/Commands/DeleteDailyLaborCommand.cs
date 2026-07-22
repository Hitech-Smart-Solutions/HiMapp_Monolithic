using MediatR;

namespace Himapp.Execution.Application.Features.DailyLabor.Commands;

public sealed record DeleteDailyLaborCommand(int Id) : IRequest<bool>;
