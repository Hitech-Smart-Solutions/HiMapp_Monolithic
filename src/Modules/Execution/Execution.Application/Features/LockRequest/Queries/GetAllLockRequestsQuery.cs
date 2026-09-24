using MediatR;
using Himapp.Execution.Application.Features.LockRequest.Models;
using System.Collections.Generic;

namespace Himapp.Execution.Application.Features.LockRequest.Queries;

public sealed record GetAllLockRequestsQuery() : IRequest<IReadOnlyCollection<LockRequestModel>>;
