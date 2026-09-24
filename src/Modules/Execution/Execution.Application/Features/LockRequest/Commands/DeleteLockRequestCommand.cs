using MediatR;

namespace Himapp.Execution.Application.Features.LockRequest.Commands;

public sealed record DeleteLockRequestCommand(int Id) : IRequest<bool>;
