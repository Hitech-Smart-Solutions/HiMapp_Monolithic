using MediatR;
using Himapp.Execution.Application.Features.LockRequest.Models;

namespace Himapp.Execution.Application.Features.LockRequest.Commands;

public sealed record CreateLockRequestCommand(CreateLockRequestRequest Request) : IRequest<LockRequestModel>;
