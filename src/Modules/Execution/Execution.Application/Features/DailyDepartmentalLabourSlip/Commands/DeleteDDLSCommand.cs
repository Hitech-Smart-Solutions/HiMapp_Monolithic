using MediatR;

namespace Himapp.Execution.Application.Features.DailyDepartmentalLabourSlip.Commands;

public sealed record DeleteDDLSCommand(AddTransactionActionHistoryDTO addTransactionActionHistoryDTO) : IRequest<bool>;
