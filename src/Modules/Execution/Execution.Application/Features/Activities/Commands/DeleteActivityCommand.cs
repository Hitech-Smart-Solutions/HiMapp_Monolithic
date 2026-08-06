using MediatR;

namespace Himapp.Execution.Application.Features.Activities.Commands;

public sealed record DeleteActivityCommand(AddTransactionActionHistoryDTO addTransactionActionHistoryDTO) : IRequest<bool>;
