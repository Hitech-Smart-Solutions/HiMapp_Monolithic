using MediatR;

namespace Himapp.Execution.Application.Features.Activities.Commands;

public sealed record CreateActivityCommand(long ProjectId, string ActivityCode, string Description, decimal ProgressPercent, DateOnly WorkDate) : IRequest<ActivityDto>;
public sealed record UpdateActivityCommand(long Id, long ProjectId, string ActivityCode, string Description, decimal ProgressPercent, DateOnly WorkDate) : IRequest<ActivityDto?>;
public sealed record DeleteActivityCommand(long Id) : IRequest<bool>;
