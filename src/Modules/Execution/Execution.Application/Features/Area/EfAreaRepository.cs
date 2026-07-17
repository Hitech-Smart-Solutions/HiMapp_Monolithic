using Himapp.Execution.Application.Features.Area.Models;
using Microsoft.EntityFrameworkCore;

namespace Himapp.Execution.Application.Features.Area;

internal sealed class EfAreaRepository : IAreaRepository
{
    private readonly DbContext _db;
    public EfAreaRepository(DbContext db) => _db = db;

    public async Task<IReadOnlyCollection<Models.AreaModel>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _db.Set<Himapp.Execution.Domain.Entities.Area>()
            .AsNoTracking()
            .Select(a => new Models.AreaModel(a.Id, a.UniqueId, a.ProjectId, a.Name, a.Code, a.IsActive, a.CreatedBy, a.CreatedDate, a.LastModifiedBy, a.LastModifiedDate))
            .ToArrayAsync(cancellationToken);
    }

    public async Task<Models.AreaModel?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var a = await _db.Set<Himapp.Execution.Domain.Entities.Area>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (a is null) return null;
        return new Models.AreaModel(a.Id, a.UniqueId, a.ProjectId, a.Name, a.Code, a.IsActive, a.CreatedBy, a.CreatedDate, a.LastModifiedBy, a.LastModifiedDate);
    }

    public async Task<Models.AreaModel> AddAsync(Models.CreateAreaRequest request, CancellationToken cancellationToken = default)
    {
        var entity = new Himapp.Execution.Domain.Entities.Area
        {
            UniqueId = Guid.NewGuid(),
            ProjectId = request.ProjectId,
            Name = request.Name,
            Code = request.Code,
            IsActive = true,
            CreatedBy = null,
            CreatedDate = DateTimeOffset.UtcNow,
            LastModifiedBy = null,
            LastModifiedDate = DateTimeOffset.UtcNow
        };
        _db.Set<Himapp.Execution.Domain.Entities.Area>().Add(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return new Models.AreaModel(entity.Id, entity.UniqueId, entity.ProjectId, entity.Name, entity.Code, entity.IsActive, entity.CreatedBy, entity.CreatedDate, entity.LastModifiedBy, entity.LastModifiedDate);
    }

    public async Task<Models.AreaModel?> UpdateAsync(long id, Models.UpdateAreaRequest request, CancellationToken cancellationToken = default)
    {
        var entity = await _db.Set<Himapp.Execution.Domain.Entities.Area>().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null) return null;
        entity.Name = request.Name;
        entity.Code = request.Code;
        entity.IsActive = request.IsActive;
        entity.LastModifiedDate = DateTimeOffset.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
        return new Models.AreaModel(entity.Id, entity.UniqueId, entity.ProjectId, entity.Name, entity.Code, entity.IsActive, entity.CreatedBy, entity.CreatedDate, entity.LastModifiedBy, entity.LastModifiedDate);
    }

    public async Task<bool> DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await _db.Set<Himapp.Execution.Domain.Entities.Area>().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null) return false;
        entity.IsActive = false;
        entity.LastModifiedDate = DateTimeOffset.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }
}
