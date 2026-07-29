using Himapp.Execution.Application.Features.ProjectActivities.Models;
using Himapp.Execution.Application.Features.ProjectActivities.Commands;
using Himapp.Execution.Application.Features.ProjectActivities.Queries;
using Himapp.Execution.Domain.Entities;
using Himapp.Execution.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Himapp.Execution.Application.Features.ProjectActivities.Handlers;

internal sealed class CreateProjectActivityCommandHandler : IRequestHandler<CreateProjectActivityCommand, ProjectActivityModel>
{
    private readonly ExecutionDbContext _db;
    public CreateProjectActivityCommandHandler(ExecutionDbContext db) => _db = db;

    public async Task<ProjectActivityModel> Handle(CreateProjectActivityCommand request, CancellationToken cancellationToken)
    {
        var r = request.Request;
        var entity = new ProjectActivity
        {
            UniqueID = Guid.NewGuid(),
            ProjectID = r.ProjectId,
            ActivityID = r.ActivityId,
            IsActive = true,
            CreatedBy = null,
            CreatedDate = DateTimeOffset.UtcNow,
            LastModifiedBy = null,
            LastModifiedDate = DateTimeOffset.UtcNow
        };

        _db.ProjectActivities.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);

        return new ProjectActivityModel(entity.ID, entity.UniqueID, entity.ProjectID, entity.ActivityID, entity.IsActive, entity.CreatedBy, entity.CreatedDate, entity.LastModifiedBy, entity.LastModifiedDate);
    }
}

internal sealed class UpdateProjectActivityCommandHandler : IRequestHandler<UpdateProjectActivityCommand, ProjectActivityModel?>
{
    private readonly ExecutionDbContext _db;
    public UpdateProjectActivityCommandHandler(ExecutionDbContext db) => _db = db;

    public async Task<ProjectActivityModel?> Handle(UpdateProjectActivityCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.ProjectActivities.FirstOrDefaultAsync(x => x.ID == request.Id, cancellationToken);
        if (entity is null) return null;

        entity.ProjectID = request.Request.ProjectId;
        entity.ActivityID = request.Request.ActivityId;
        entity.IsActive = request.Request.IsActive;
        entity.LastModifiedDate = DateTimeOffset.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);

        return new ProjectActivityModel(entity.ID, entity.UniqueID, entity.ProjectID, entity.ActivityID, entity.IsActive, entity.CreatedBy, entity.CreatedDate, entity.LastModifiedBy, entity.LastModifiedDate);
    }
}

internal sealed class DeleteProjectActivityCommandHandler : IRequestHandler<DeleteProjectActivityCommand, bool>
{
    private readonly ExecutionDbContext _db;
    public DeleteProjectActivityCommandHandler(ExecutionDbContext db) => _db = db;

    public async Task<bool> Handle(DeleteProjectActivityCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.ProjectActivities.FirstOrDefaultAsync(x => x.ID == request.Id, cancellationToken);
        if (entity is null) return false;

        entity.IsActive = false;
        entity.LastModifiedDate = DateTimeOffset.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }
}

internal sealed class GetAllProjectActivitiesQueryHandler : IRequestHandler<GetAllProjectActivitiesQuery, IReadOnlyCollection<ProjectActivityModel>>
{
    private readonly ExecutionDbContext _db;
    public GetAllProjectActivitiesQueryHandler(ExecutionDbContext db) => _db = db;

    public async Task<IReadOnlyCollection<ProjectActivityModel>> Handle(GetAllProjectActivitiesQuery request, CancellationToken cancellationToken)
    {
        return await _db.ProjectActivities
            .AsNoTracking()
            .Select(p => new ProjectActivityModel(p.ID, p.UniqueID, p.ProjectID, p.ActivityID, p.IsActive, p.CreatedBy, p.CreatedDate, p.LastModifiedBy, p.LastModifiedDate))
            .ToArrayAsync(cancellationToken);
    }
}

internal sealed class GetProjectActivityByIdQueryHandler : IRequestHandler<GetProjectActivityByIdQuery, ProjectActivityModel?>
{
    private readonly ExecutionDbContext _db;
    public GetProjectActivityByIdQueryHandler(ExecutionDbContext db) => _db = db;

    public async Task<ProjectActivityModel?> Handle(GetProjectActivityByIdQuery request, CancellationToken cancellationToken)
    {
        var p = await _db.ProjectActivities.AsNoTracking().FirstOrDefaultAsync(x => x.ID == request.Id, cancellationToken);
        if (p is null) return null;
        return new ProjectActivityModel(p.ID, p.UniqueID, p.ProjectID, p.ActivityID, p.IsActive, p.CreatedBy, p.CreatedDate, p.LastModifiedBy, p.LastModifiedDate);
    }
}

