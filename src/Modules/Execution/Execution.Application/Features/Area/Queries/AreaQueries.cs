using MediatR;
using Himapp.Execution.Application.Features.Area.Models;

namespace Himapp.Execution.Application.Features.Area.Queries;

public sealed record GetAllAreasQuery : IRequest<IReadOnlyCollection<AreaModel>>;
public sealed record GetAreaByIdQuery(long Id) : IRequest<AreaModel?>;
