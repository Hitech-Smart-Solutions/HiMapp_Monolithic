using MediatR;
using Himapp.Execution.Application.Features.RateMaster.Models;

namespace Himapp.Execution.Application.Features.RateMaster.Queries;

public sealed record GetAllRateMastersQuery : IRequest<IReadOnlyCollection<RateMasterModel>>;
public sealed record GetRateMasterByIdQuery(long Id) : IRequest<RateMasterModel?>;
