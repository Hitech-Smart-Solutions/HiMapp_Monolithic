using Himapp.Execution.Application.Features.Uom.Models;
using Microsoft.EntityFrameworkCore;

namespace Himapp.Execution.Application.Features.Uom;

internal sealed class EfUomRepository : IUomRepository
{
    private readonly DbContext _db;
    public EfUomRepository(DbContext db) => _db = db;

    public async Task<IReadOnlyCollection<Models.UomModel>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _db.Set<Himapp.Execution.Domain.Entities.UOM>()
            .AsNoTracking()
            .Select(u => new Models.UomModel(u.Id, u.UniqueId, u.CompanyId, u.Name, u.Code, u.IsActive, u.CreatedBy, u.CreatedDate, u.LastModifiedBy, u.LastModifiedDate))
            .ToArrayAsync(cancellationToken);
    }

    public async Task<Models.UomModel?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var u = await _db.Set<Himapp.Execution.Domain.Entities.UOM>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (u is null) return null;
        return new Models.UomModel(u.Id, u.UniqueId, u.CompanyId, u.Name, u.Code, u.IsActive, u.CreatedBy, u.CreatedDate, u.LastModifiedBy, u.LastModifiedDate);
    }

    public async Task<Models.UomModel> AddAsync(Models.CreateUomRequest request, CancellationToken cancellationToken = default)
    {
        var entity = new Himapp.Execution.Domain.Entities.UOM
        {
            UniqueId = Guid.NewGuid(),
            CompanyId = request.CompanyId,
            Name = request.Name,
            Code = request.Code,
            IsActive = true,
            CreatedBy = null,
            CreatedDate = DateTimeOffset.UtcNow,
            LastModifiedBy = null,
            LastModifiedDate = DateTimeOffset.UtcNow
        };
        _db.Set<Himapp.Execution.Domain.Entities.UOM>().Add(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return new Models.UomModel(entity.Id, entity.UniqueId, entity.CompanyId, entity.Name, entity.Code, entity.IsActive, entity.CreatedBy, entity.CreatedDate, entity.LastModifiedBy, entity.LastModifiedDate);
    }

    public async Task<Models.UomModel?> UpdateAsync(long id, Models.UpdateUomRequest request, CancellationToken cancellationToken = default)
    {
        var entity = await _db.Set<Himapp.Execution.Domain.Entities.UOM>().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null) return null;
        entity.Name = request.Name;
        entity.Code = request.Code;
        entity.IsActive = request.IsActive;
        entity.LastModifiedDate = DateTimeOffset.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
        return new Models.UomModel(entity.Id, entity.UniqueId, entity.CompanyId, entity.Name, entity.Code, entity.IsActive, entity.CreatedBy, entity.CreatedDate, entity.LastModifiedBy, entity.LastModifiedDate);
    }

    public async Task<bool> DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await _db.Set<Himapp.Execution.Domain.Entities.UOM>().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null) return false;
        entity.IsActive = false;
        entity.LastModifiedDate = DateTimeOffset.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }
}
