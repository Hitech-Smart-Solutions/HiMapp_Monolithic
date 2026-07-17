using Himapp.Execution.Application.Features.DailyLabor.Models;
using Microsoft.EntityFrameworkCore;

namespace Himapp.Execution.Application.Features.DailyLabor;

internal sealed class EfDailyLaborRepository : IDailyLaborRepository
{
    private readonly DbContext _db;
    public EfDailyLaborRepository(DbContext db) => _db = db;

    public async Task<IReadOnlyCollection<Models.DailyLaborModel>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _db.Set<Himapp.Execution.Domain.Entities.DailyLabor>()
            .AsNoTracking()
            .Select(d => new Models.DailyLaborModel(d.Id, d.UniqueId, d.ProjectId, d.ReportDate, d.Remarks, d.Status, d.IsActive, d.CreatedBy, d.CreatedDate, d.LastModifiedBy, d.LastModifiedDate))
            .ToArrayAsync(cancellationToken);
    }

    public async Task<Models.DailyLaborModel?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var d = await _db.Set<Himapp.Execution.Domain.Entities.DailyLabor>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (d is null) return null;
        return new Models.DailyLaborModel(d.Id, d.UniqueId, d.ProjectId, d.ReportDate, d.Remarks, d.Status, d.IsActive, d.CreatedBy, d.CreatedDate, d.LastModifiedBy, d.LastModifiedDate);
    }

    public async Task<Models.DailyLaborModel> AddAsync(Models.CreateDailyLaborRequest request, CancellationToken cancellationToken = default)
    {
        var entity = new Himapp.Execution.Domain.Entities.DailyLabor
        {
            UniqueId = Guid.NewGuid(),
            ProjectId = request.ProjectId,
            ReportDate = request.ReportDate,
            Remarks = request.Remarks,
            Status = "DRAFT",
            IsActive = true,
            CreatedBy = null,
            CreatedDate = DateTimeOffset.UtcNow,
            LastModifiedBy = null,
            LastModifiedDate = DateTimeOffset.UtcNow
        };
        _db.Set<Himapp.Execution.Domain.Entities.DailyLabor>().Add(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return new Models.DailyLaborModel(entity.Id, entity.UniqueId, entity.ProjectId, entity.ReportDate, entity.Remarks, entity.Status, entity.IsActive, entity.CreatedBy, entity.CreatedDate, entity.LastModifiedBy, entity.LastModifiedDate);
    }

    public async Task<Models.DailyLaborModel?> UpdateAsync(long id, Models.UpdateDailyLaborRequest request, CancellationToken cancellationToken = default)
    {
        var entity = await _db.Set<Himapp.Execution.Domain.Entities.DailyLabor>().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null) return null;
        entity.Remarks = request.Remarks ?? entity.Remarks;
        entity.Status = request.Status;
        entity.IsActive = request.IsActive;
        entity.LastModifiedDate = DateTimeOffset.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
        return new Models.DailyLaborModel(entity.Id, entity.UniqueId, entity.ProjectId, entity.ReportDate, entity.Remarks, entity.Status, entity.IsActive, entity.CreatedBy, entity.CreatedDate, entity.LastModifiedBy, entity.LastModifiedDate);
    }

    public async Task<bool> DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await _db.Set<Himapp.Execution.Domain.Entities.DailyLabor>().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null) return false;
        entity.IsActive = false;
        entity.LastModifiedDate = DateTimeOffset.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }
}
