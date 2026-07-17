using Himapp.Execution.Application.Features.RateMaster.Models;
using Himapp.Execution.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Himapp.Execution.Application.Features.RateMaster;

internal sealed class EfRateMasterRepository : IRateMasterRepository
{
    private readonly DbContext _db;
    public EfRateMasterRepository(DbContext db) => _db = db;

    public async Task<IReadOnlyCollection<RateMasterModel>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _db.Set<Himapp.Execution.Domain.Entities.RateMaster>()
            .AsNoTracking()
            .Select(r => new RateMasterModel(r.Id, r.UniqueId, r.ProjectId, r.ActivityId, r.Rate, 0, r.EffectiveFrom, r.IsActive, r.CreatedBy, r.CreatedDate, r.LastModifiedBy, r.LastModifiedDate))
            .ToArrayAsync(cancellationToken);
    }

    public async Task<RateMasterModel?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var r = await _db.Set<Himapp.Execution.Domain.Entities.RateMaster>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (r is null) return null;
        return new RateMasterModel(r.Id, r.UniqueId, r.ProjectId, r.ActivityId, r.Rate, 0, r.EffectiveFrom, r.IsActive, r.CreatedBy, r.CreatedDate, r.LastModifiedBy, r.LastModifiedDate);
    }

    public async Task<RateMasterModel> AddAsync(CreateRateMasterRequest model, CancellationToken cancellationToken = default)
    {
        var entity = new Himapp.Execution.Domain.Entities.RateMaster
        {
            UniqueId = Guid.NewGuid(),
            ProjectId = model.ProjectId,
            ActivityId = model.ActivityId,
            Rate = model.Rate,
            Uom = string.Empty,
            EffectiveFrom = model.EffectiveFrom,
            IsActive = true,
            CreatedBy = null,
            CreatedDate = DateTimeOffset.UtcNow,
            LastModifiedBy = null,
            LastModifiedDate = DateTimeOffset.UtcNow
        };

        _db.Set<Himapp.Execution.Domain.Entities.RateMaster>().Add(entity);
        await _db.SaveChangesAsync(cancellationToken);

        return new RateMasterModel(entity.Id, entity.UniqueId, entity.ProjectId, entity.ActivityId, entity.Rate, 0, entity.EffectiveFrom, entity.IsActive, entity.CreatedBy, entity.CreatedDate, entity.LastModifiedBy, entity.LastModifiedDate);
    }

    public async Task<RateMasterModel?> UpdateAsync(long id, UpdateRateMasterRequest model, CancellationToken cancellationToken = default)
    {
        var entity = await _db.Set<Himapp.Execution.Domain.Entities.RateMaster>().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null) return null;

        entity.ProjectId = model.ProjectId;
        entity.ActivityId = model.ActivityId;
        entity.Rate = model.Rate;
        entity.Uom = string.Empty;
        entity.EffectiveFrom = model.EffectiveFrom;
        entity.IsActive = model.IsActive;
        entity.LastModifiedDate = DateTimeOffset.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);

        return new RateMasterModel(entity.Id, entity.UniqueId, entity.ProjectId, entity.ActivityId, entity.Rate, 0, entity.EffectiveFrom, entity.IsActive, entity.CreatedBy, entity.CreatedDate, entity.LastModifiedBy, entity.LastModifiedDate);
    }

    public async Task<bool> DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await _db.Set<Himapp.Execution.Domain.Entities.RateMaster>().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null) return false;

        entity.IsActive = false;
        entity.LastModifiedDate = DateTimeOffset.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }
}
