using MediatR;
using Himapp.Execution.Application.Features.Planning.Models;

namespace Himapp.Execution.Application.Features.Planning.Queries;

public sealed record GetAllPlanningsQuery : IRequest<IReadOnlyCollection<PlanningModel>>;
public sealed record GetPlanningByIdQuery(long Id) : IRequest<PlanningModel?>;
