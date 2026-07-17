using Himapp.Execution.Application.Features.ProjectActivities.Models;
using Himapp.Execution.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Himapp.Execution.Application.Features.ProjectActivities;

internal sealed class EfProjectActivityRepository : IProjectActivityRepository
{
    private readonly DbContext _db;
    public EfProjectActivityRepository(DbContext db) => _db = db;

    public async Task<IReadOnlyCollection<ProjectActivityModel>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _db.Set<ProjectActivity>()
            .AsNoTracking()
            .Select(p => new ProjectActivityModel(p.Id, p.UniqueId, p.ProjectId, p.ActivityId, p.IsActive, p.CreatedBy, p.CreatedDate, p.LastModifiedBy, p.LastModifiedDate))
            .ToArrayAsync(cancellationToken);
    }

    public async Task<ProjectActivityModel?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var p = await _db.Set<ProjectActivity>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (p is null) return null;
        return new ProjectActivityModel(p.Id, p.UniqueId, p.ProjectId, p.ActivityId, p.IsActive, p.CreatedBy, p.CreatedDate, p.LastModifiedBy, p.LastModifiedDate);
    }

    public async Task<ProjectActivityModel> AddAsync(CreateProjectActivityRequest model, CancellationToken cancellationToken = default)
    {
        var entity = new ProjectActivity
        {
            UniqueId = Guid.NewGuid(),
            ProjectId = model.ProjectId,
            ActivityId = model.ActivityId,
            IsActive = true,
            CreatedBy = null,
            CreatedDate = DateTimeOffset.UtcNow,
            LastModifiedBy = null,
            LastModifiedDate = DateTimeOffset.UtcNow
        };

        _db.Set<ProjectActivity>().Add(entity);
        await _db.SaveChangesAsync(cancellationToken);

        return new ProjectActivityModel(entity.Id, entity.UniqueId, entity.ProjectId, entity.ActivityId, entity.IsActive, entity.CreatedBy, entity.CreatedDate, entity.LastModifiedBy, entity.LastModifiedDate);
    }

    public async Task<ProjectActivityModel?> UpdateAsync(long id, UpdateProjectActivityRequest model, CancellationToken cancellationToken = default)
    {
        var entity = await _db.Set<ProjectActivity>().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null) return null;

        entity.ProjectId = model.ProjectId;
        entity.ActivityId = model.ActivityId;
        entity.IsActive = model.IsActive;
        entity.LastModifiedDate = DateTimeOffset.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);

        return new ProjectActivityModel(entity.Id, entity.UniqueId, entity.ProjectId, entity.ActivityId, entity.IsActive, entity.CreatedBy, entity.CreatedDate, entity.LastModifiedBy, entity.LastModifiedDate);
    }

    public async Task<bool> DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await _db.Set<ProjectActivity>().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null) return false;

        entity.IsActive = false;
        entity.LastModifiedDate = DateTimeOffset.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }
}
