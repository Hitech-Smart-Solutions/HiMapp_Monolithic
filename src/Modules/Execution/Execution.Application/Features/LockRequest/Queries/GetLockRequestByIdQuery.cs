using MediatR;
using Himapp.Execution.Application.Features.LockRequest.Models;

namespace Himapp.Execution.Application.Features.LockRequest.Queries;

public sealed record GetLockRequestByIdQuery(int Id) : IRequest<LockRequestModel?>;
