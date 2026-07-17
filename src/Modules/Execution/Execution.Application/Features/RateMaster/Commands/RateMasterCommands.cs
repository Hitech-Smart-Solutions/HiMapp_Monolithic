using MediatR;
using Himapp.Execution.Application.Features.RateMaster.Models;

namespace Himapp.Execution.Application.Features.RateMaster.Commands;

public sealed record CreateRateMasterCommand(CreateRateMasterRequest Request) : IRequest<RateMasterModel>;
public sealed record UpdateRateMasterCommand(long Id, UpdateRateMasterRequest Request) : IRequest<RateMasterModel?>;
public sealed record DeleteRateMasterCommand(long Id) : IRequest<bool>;
