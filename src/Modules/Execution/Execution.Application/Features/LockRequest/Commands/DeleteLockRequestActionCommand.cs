using MediatR;

namespace Himapp.Execution.Application.Features.LockRequest.Commands;

public sealed record DeleteLockRequestActionCommand(AddTransactionActionHistoryDTO addTransactionActionHistoryDTO) : IRequest<bool>;
