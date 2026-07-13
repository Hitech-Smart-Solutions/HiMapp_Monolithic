using Himapp.Execution.Application.Features.Activities.Commands;
using Himapp.Execution.Application.Features.Activities.Queries;
using MediatR;

namespace Himapp.Execution.Application.Features.Activities.Handlers;

internal sealed class CreateActivityCommandHandler : IRequestHandler<CreateActivityCommand, ActivityDto>
{
    private readonly IActivityRepository _repository;
    public CreateActivityCommandHandler(IActivityRepository repository) => _repository = repository;
    public Task<ActivityDto> Handle(CreateActivityCommand request, CancellationToken cancellationToken) =>
        _repository.AddAsync(new ActivityDto(0, request.ProjectId, request.ActivityCode, request.Description, request.ProgressPercent, request.WorkDate), cancellationToken);
}

internal sealed class UpdateActivityCommandHandler : IRequestHandler<UpdateActivityCommand, ActivityDto?>
{
    private readonly IActivityRepository _repository;
    public UpdateActivityCommandHandler(IActivityRepository repository) => _repository = repository;
    public Task<ActivityDto?> Handle(UpdateActivityCommand request, CancellationToken cancellationToken) =>
        _repository.UpdateAsync(new ActivityDto(request.Id, request.ProjectId, request.ActivityCode, request.Description, request.ProgressPercent, request.WorkDate), cancellationToken);
}

internal sealed class DeleteActivityCommandHandler : IRequestHandler<DeleteActivityCommand, bool>
{
    private readonly IActivityRepository _repository;
    public DeleteActivityCommandHandler(IActivityRepository repository) => _repository = repository;
    public Task<bool> Handle(DeleteActivityCommand request, CancellationToken cancellationToken) =>
        _repository.DeleteAsync(request.Id, cancellationToken);
}

internal sealed class GetAllActivitiesQueryHandler : IRequestHandler<GetAllActivitiesQuery, IReadOnlyCollection<ActivityDto>>
{
    private readonly IActivityRepository _repository;
    public GetAllActivitiesQueryHandler(IActivityRepository repository) => _repository = repository;
    public Task<IReadOnlyCollection<ActivityDto>> Handle(GetAllActivitiesQuery request, CancellationToken cancellationToken) =>
        _repository.GetAllAsync(cancellationToken);
}

internal sealed class GetActivityByIdQueryHandler : IRequestHandler<GetActivityByIdQuery, ActivityDto?>
{
    private readonly IActivityRepository _repository;
    public GetActivityByIdQueryHandler(IActivityRepository repository) => _repository = repository;
    public Task<ActivityDto?> Handle(GetActivityByIdQuery request, CancellationToken cancellationToken) =>
        _repository.GetByIdAsync(request.Id, cancellationToken);
}
