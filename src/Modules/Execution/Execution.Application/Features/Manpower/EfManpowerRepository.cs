using Himapp.Execution.Application.Features.Manpower.Models;
using Microsoft.EntityFrameworkCore;

namespace Himapp.Execution.Application.Features.Manpower;

internal sealed class EfManpowerRepository : IManpowerRepository
{
    private readonly DbContext _db;
    public EfManpowerRepository(DbContext db) => _db = db;

    public async Task<IReadOnlyCollection<Models.ManpowerModel>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _db.Set<Himapp.Execution.Domain.Entities.Manpower>()
            .AsNoTracking()
            .Select(m => new Models.ManpowerModel(m.Id, m.UniqueId, m.ProjectId, m.EntryDate, m.Shift, m.Remarks, m.Status, m.IsActive, m.CreatedBy, m.CreatedDate, m.LastModifiedBy, m.LastModifiedDate))
            .ToArrayAsync(cancellationToken);
    }

    public async Task<Models.ManpowerModel?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var m = await _db.Set<Himapp.Execution.Domain.Entities.Manpower>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (m is null) return null;
        return new Models.ManpowerModel(m.Id, m.UniqueId, m.ProjectId, m.EntryDate, m.Shift, m.Remarks, m.Status, m.IsActive, m.CreatedBy, m.CreatedDate, m.LastModifiedBy, m.LastModifiedDate);
    }

    public async Task<Models.ManpowerModel> AddAsync(Models.CreateManpowerRequest request, CancellationToken cancellationToken = default)
    {
        var entity = new Himapp.Execution.Domain.Entities.Manpower
        {
            UniqueId = Guid.NewGuid(),
            ProjectId = request.ProjectId,
            EntryDate = request.EntryDate,
            Shift = request.Shift,
            Remarks = request.Remarks,
            Status = "DRAFT",
            IsActive = true,
            CreatedBy = null,
            CreatedDate = DateTimeOffset.UtcNow,
            LastModifiedBy = null,
            LastModifiedDate = DateTimeOffset.UtcNow
        };
        _db.Set<Himapp.Execution.Domain.Entities.Manpower>().Add(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return new Models.ManpowerModel(entity.Id, entity.UniqueId, entity.ProjectId, entity.EntryDate, entity.Shift, entity.Remarks, entity.Status, entity.IsActive, entity.CreatedBy, entity.CreatedDate, entity.LastModifiedBy, entity.LastModifiedDate);
    }

    public async Task<Models.ManpowerModel?> UpdateAsync(long id, Models.UpdateManpowerRequest request, CancellationToken cancellationToken = default)
    {
        var entity = await _db.Set<Himapp.Execution.Domain.Entities.Manpower>().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null) return null;
        entity.Remarks = request.Remarks ?? entity.Remarks;
        entity.Status = request.Status;
        entity.IsActive = request.IsActive;
        entity.LastModifiedDate = DateTimeOffset.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
        return new Models.ManpowerModel(entity.Id, entity.UniqueId, entity.ProjectId, entity.EntryDate, entity.Shift, entity.Remarks, entity.Status, entity.IsActive, entity.CreatedBy, entity.CreatedDate, entity.LastModifiedBy, entity.LastModifiedDate);
    }

    public async Task<bool> DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await _db.Set<Himapp.Execution.Domain.Entities.Manpower>().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null) return false;
        entity.IsActive = false;
        entity.LastModifiedDate = DateTimeOffset.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }
}
