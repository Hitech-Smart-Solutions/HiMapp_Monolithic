using MediatR;
using Himapp.Execution.Application.Features.Area.Models;

namespace Himapp.Execution.Application.Features.Area.Commands;

public sealed record CreateAreaCommand(CreateAreaRequest Request) : IRequest<AreaModel>;
public sealed record UpdateAreaCommand(long Id, UpdateAreaRequest Request) : IRequest<AreaModel?>;
public sealed record DeleteAreaCommand(long Id) : IRequest<bool>;
