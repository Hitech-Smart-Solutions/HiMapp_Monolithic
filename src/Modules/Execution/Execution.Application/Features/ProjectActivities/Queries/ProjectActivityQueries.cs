using MediatR;
using Himapp.Execution.Application.Features.ProjectActivities.Models;

namespace Himapp.Execution.Application.Features.ProjectActivities.Queries;

public sealed record GetAllProjectActivitiesQuery : IRequest<IReadOnlyCollection<ProjectActivityModel>>;
public sealed record GetProjectActivityByIdQuery(long Id) : IRequest<ProjectActivityModel?>;
