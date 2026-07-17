using MediatR;
using Himapp.Execution.Application.Features.Uom.Models;

namespace Himapp.Execution.Application.Features.Uom.Queries;

public sealed record GetAllUomsQuery : IRequest<IReadOnlyCollection<UomModel>>;
public sealed record GetUomByIdQuery(long Id) : IRequest<UomModel?>;
