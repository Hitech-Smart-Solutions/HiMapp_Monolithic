using Himapp.Execution.Application.Features.ProjectActivities.Models;
using Himapp.Execution.Application.Features.ProjectActivities.Commands;
using Himapp.Execution.Application.Features.ProjectActivities.Queries;
using Himapp.Execution.Domain.Entities;
using Himapp.Execution.Contracts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Himapp.Execution.Application.Features.ProjectActivities.Handlers;

internal sealed class ProjectActivityHandlers :
    IRequestHandler<CreateProjectActivityCommand, ProjectActivityModel>,
    IRequestHandler<UpdateProjectActivityCommand, ProjectActivityModel?>,
    IRequestHandler<DeleteProjectActivityCommand, bool>,
    IRequestHandler<GetAllProjectActivitiesQuery, IReadOnlyCollection<ProjectActivityModel>>,
    IRequestHandler<GetProjectActivityByIdQuery, ProjectActivityModel?>
{
    private readonly IExecutionDbContext _db;
    public ProjectActivityHandlers(IExecutionDbContext db) => _db = db;

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

        _db.Set<ProjectActivity>().Add(entity);
        await _db.SaveChangesAsync(cancellationToken);

        return new ProjectActivityModel(entity.ID, entity.UniqueID, entity.ProjectID, entity.ActivityID, entity.IsActive, entity.CreatedBy, entity.CreatedDate, entity.LastModifiedBy, entity.LastModifiedDate);
    }

    public async Task<ProjectActivityModel?> Handle(UpdateProjectActivityCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.Set<ProjectActivity>().FirstOrDefaultAsync(x => x.ID == request.Id, cancellationToken);
        if (entity is null) return null;

        entity.ProjectID = request.Request.ProjectId;
        entity.ActivityID = request.Request.ActivityId;
        entity.IsActive = request.Request.IsActive;
        entity.LastModifiedDate = DateTimeOffset.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);

        return new ProjectActivityModel(entity.ID, entity.UniqueID, entity.ProjectID, entity.ActivityID, entity.IsActive, entity.CreatedBy, entity.CreatedDate, entity.LastModifiedBy, entity.LastModifiedDate);
    }

    public async Task<bool> Handle(DeleteProjectActivityCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.Set<ProjectActivity>().FirstOrDefaultAsync(x => x.ID == request.Id, cancellationToken);
        if (entity is null) return false;

        entity.IsActive = false;
        entity.LastModifiedDate = DateTimeOffset.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<IReadOnlyCollection<ProjectActivityModel>> Handle(GetAllProjectActivitiesQuery request, CancellationToken cancellationToken)
    {
        return await _db.Set<ProjectActivity>()
            .AsNoTracking()
            .Select(p => new ProjectActivityModel(p.ID, p.UniqueID, p.ProjectID, p.ActivityID, p.IsActive, p.CreatedBy, p.CreatedDate, p.LastModifiedBy, p.LastModifiedDate))
            .ToArrayAsync(cancellationToken);
    }

    public async Task<ProjectActivityModel?> Handle(GetProjectActivityByIdQuery request, CancellationToken cancellationToken)
    {
        var p = await _db.Set<ProjectActivity>().AsNoTracking().FirstOrDefaultAsync(x => x.ID == request.Id, cancellationToken);
        if (p is null) return null;
        return new ProjectActivityModel(p.ID, p.UniqueID, p.ProjectID, p.ActivityID, p.IsActive, p.CreatedBy, p.CreatedDate, p.LastModifiedBy, p.LastModifiedDate);
    }
}

