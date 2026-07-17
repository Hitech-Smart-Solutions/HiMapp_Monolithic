using Himapp.Execution.Application.Features.ProjectActivities.Models;
using Himapp.Execution.Application.Features.ProjectActivities.Commands;
using Himapp.Execution.Application.Features.ProjectActivities.Queries;
using MediatR;

namespace Himapp.Execution.Application.Features.ProjectActivities.Handlers;

internal sealed class CreateProjectActivityCommandHandler : IRequestHandler<CreateProjectActivityCommand, ProjectActivityModel>
{
    private readonly Features.ProjectActivities.IProjectActivityRepository _repo;
    public CreateProjectActivityCommandHandler(Features.ProjectActivities.IProjectActivityRepository repo) => _repo = repo;
    public Task<ProjectActivityModel> Handle(CreateProjectActivityCommand request, CancellationToken cancellationToken) => _repo.AddAsync(request.Request, cancellationToken);
}

internal sealed class UpdateProjectActivityCommandHandler : IRequestHandler<UpdateProjectActivityCommand, ProjectActivityModel?>
{
    private readonly Features.ProjectActivities.IProjectActivityRepository _repo;
    public UpdateProjectActivityCommandHandler(Features.ProjectActivities.IProjectActivityRepository repo) => _repo = repo;
    public Task<ProjectActivityModel?> Handle(UpdateProjectActivityCommand request, CancellationToken cancellationToken) => _repo.UpdateAsync(request.Id, request.Request, cancellationToken);
}

internal sealed class DeleteProjectActivityCommandHandler : IRequestHandler<DeleteProjectActivityCommand, bool>
{
    private readonly Features.ProjectActivities.IProjectActivityRepository _repo;
    public DeleteProjectActivityCommandHandler(Features.ProjectActivities.IProjectActivityRepository repo) => _repo = repo;
    public Task<bool> Handle(DeleteProjectActivityCommand request, CancellationToken cancellationToken) => _repo.DeleteAsync(request.Id, cancellationToken);
}

internal sealed class GetAllProjectActivitiesQueryHandler : IRequestHandler<GetAllProjectActivitiesQuery, IReadOnlyCollection<ProjectActivityModel>>
{
    private readonly Features.ProjectActivities.IProjectActivityRepository _repo;
    public GetAllProjectActivitiesQueryHandler(Features.ProjectActivities.IProjectActivityRepository repo) => _repo = repo;
    public Task<IReadOnlyCollection<ProjectActivityModel>> Handle(GetAllProjectActivitiesQuery request, CancellationToken cancellationToken) => _repo.GetAllAsync(cancellationToken);
}

internal sealed class GetProjectActivityByIdQueryHandler : IRequestHandler<GetProjectActivityByIdQuery, ProjectActivityModel?>
{
    private readonly Features.ProjectActivities.IProjectActivityRepository _repo;
    public GetProjectActivityByIdQueryHandler(Features.ProjectActivities.IProjectActivityRepository repo) => _repo = repo;
    public Task<ProjectActivityModel?> Handle(GetProjectActivityByIdQuery request, CancellationToken cancellationToken) => _repo.GetByIdAsync(request.Id, cancellationToken);
}
