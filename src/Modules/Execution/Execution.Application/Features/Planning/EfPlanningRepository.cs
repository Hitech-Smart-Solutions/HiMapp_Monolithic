using Himapp.Execution.Application.Features.Planning.Models;
using Microsoft.EntityFrameworkCore;

namespace Himapp.Execution.Application.Features.Planning;

internal sealed class EfPlanningRepository : IPlanningRepository
{
    private readonly DbContext _db;
    public EfPlanningRepository(DbContext db) => _db = db;

    public async Task<IReadOnlyCollection<Models.PlanningModel>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _db.Set<Himapp.Execution.Domain.Entities.Planning>()
            .AsNoTracking()
            .Select(p => new Models.PlanningModel(
                p.Id,
                p.UniqueId,
                p.ProjectId,
                p.PlanType,
                p.StartDate,
                p.EndDate,
                p.Remarks,
                p.Status,
                p.IsActive,
                p.CreatedBy,
                p.CreatedDate,
                p.LastModifiedBy,
                p.LastModifiedDate))
            .ToArrayAsync(cancellationToken);
    }

    public async Task<Models.PlanningModel?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var p = await _db.Set<Himapp.Execution.Domain.Entities.Planning>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (p is null) return null;
        return new Models.PlanningModel(p.Id, p.UniqueId, p.ProjectId, p.PlanType, p.StartDate, p.EndDate, p.Remarks, p.Status, p.IsActive, p.CreatedBy, p.CreatedDate, p.LastModifiedBy, p.LastModifiedDate);
    }

    public async Task<Models.PlanningModel> AddAsync(Models.CreatePlanningRequest request, CancellationToken cancellationToken = default)
    {
        var entity = new Himapp.Execution.Domain.Entities.Planning
        {
            UniqueId = Guid.NewGuid(),
            ProjectId = request.ProjectId,
            PlanType = request.PlanType,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Remarks = request.Remarks,
            Status = "DRAFT",
            IsActive = true,
            CreatedBy = null,
            CreatedDate = DateTimeOffset.UtcNow,
            LastModifiedBy = null,
            LastModifiedDate = DateTimeOffset.UtcNow
        };
        _db.Set<Himapp.Execution.Domain.Entities.Planning>().Add(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return new Models.PlanningModel(entity.Id, entity.UniqueId, entity.ProjectId, entity.PlanType, entity.StartDate, entity.EndDate, entity.Remarks, entity.Status, entity.IsActive, entity.CreatedBy, entity.CreatedDate, entity.LastModifiedBy, entity.LastModifiedDate);
    }

    public async Task<Models.PlanningModel?> UpdateAsync(long id, Models.UpdatePlanningRequest request, CancellationToken cancellationToken = default)
    {
        var entity = await _db.Set<Himapp.Execution.Domain.Entities.Planning>().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null) return null;
        entity.Remarks = request.Remarks ?? entity.Remarks;
        entity.Status = request.Status;
        entity.IsActive = request.IsActive;
        entity.LastModifiedDate = DateTimeOffset.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
        return new Models.PlanningModel(entity.Id, entity.UniqueId, entity.ProjectId, entity.PlanType, entity.StartDate, entity.EndDate, entity.Remarks, entity.Status, entity.IsActive, entity.CreatedBy, entity.CreatedDate, entity.LastModifiedBy, entity.LastModifiedDate);
    }

    public async Task<bool> DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await _db.Set<Himapp.Execution.Domain.Entities.Planning>().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null) return false;
        entity.IsActive = false;
        entity.LastModifiedDate = DateTimeOffset.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }
}
