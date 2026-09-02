using MediatR;

namespace Himapp.Execution.Application.Features.DailyLabor.Commands;

public sealed record DeleteDailyLaborActionCommand(AddTransactionActionHistoryDTO addTransactionActionHistoryDTO) : IRequest<bool>;
