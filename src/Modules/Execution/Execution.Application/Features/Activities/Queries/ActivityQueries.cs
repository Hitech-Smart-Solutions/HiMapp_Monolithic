using MediatR;

namespace Himapp.Execution.Application.Features.Activities.Queries;

public sealed record GetAllActivitiesQuery : IRequest<IReadOnlyCollection<ActivityDto>>;
public sealed record GetActivityByIdQuery(long Id) : IRequest<ActivityDto?>;
