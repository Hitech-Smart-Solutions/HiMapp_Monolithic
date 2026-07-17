using Himapp.Execution.Application.Features.DailyProgress.Models;
using Microsoft.EntityFrameworkCore;

namespace Himapp.Execution.Application.Features.DailyProgress;

internal sealed class EfDailyProgressRepository : IDailyProgressRepository
{
    private readonly DbContext _db;
    public EfDailyProgressRepository(DbContext db) => _db = db;

    public async Task<IReadOnlyCollection<Models.DailyProgressModel>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _db.Set<Himapp.Execution.Domain.Entities.DailyProgress>()
            .AsNoTracking()
            .Select(d => new Models.DailyProgressModel(d.Id, d.UniqueId, d.ProjectId, d.ReportDate, d.Hindrances, d.HindranceAudioUrl, d.NextDayPlan, d.Remarks, d.TotalAmount, d.Status, d.IsActive, d.CreatedBy, d.CreatedDate, d.LastModifiedBy, d.LastModifiedDate))
            .ToArrayAsync(cancellationToken);
    }

    public async Task<Models.DailyProgressModel?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var d = await _db.Set<Himapp.Execution.Domain.Entities.DailyProgress>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (d is null) return null;
        return new Models.DailyProgressModel(d.Id, d.UniqueId, d.ProjectId, d.ReportDate, d.Hindrances, d.HindranceAudioUrl, d.NextDayPlan, d.Remarks, d.TotalAmount, d.Status, d.IsActive, d.CreatedBy, d.CreatedDate, d.LastModifiedBy, d.LastModifiedDate);
    }

    public async Task<Models.DailyProgressModel> AddAsync(Models.CreateDailyProgressRequest request, CancellationToken cancellationToken = default)
    {
        var entity = new Himapp.Execution.Domain.Entities.DailyProgress
        {
            UniqueId = Guid.NewGuid(),
            ProjectId = request.ProjectId,
            ReportDate = request.ReportDate,
            Hindrances = request.Hindrances,
            NextDayPlan = request.NextDayPlan,
            Remarks = request.Remarks,
            TotalAmount = 0m,
            Status = "DRAFT",
            IsActive = true,
            CreatedBy = null,
            CreatedDate = DateTimeOffset.UtcNow,
            LastModifiedBy = null,
            LastModifiedDate = DateTimeOffset.UtcNow
        };
        _db.Set<Himapp.Execution.Domain.Entities.DailyProgress>().Add(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return new Models.DailyProgressModel(entity.Id, entity.UniqueId, entity.ProjectId, entity.ReportDate, entity.Hindrances, entity.HindranceAudioUrl, entity.NextDayPlan, entity.Remarks, entity.TotalAmount, entity.Status, entity.IsActive, entity.CreatedBy, entity.CreatedDate, entity.LastModifiedBy, entity.LastModifiedDate);
    }

    public async Task<Models.DailyProgressModel?> UpdateAsync(long id, Models.UpdateDailyProgressRequest request, CancellationToken cancellationToken = default)
    {
        var entity = await _db.Set<Himapp.Execution.Domain.Entities.DailyProgress>().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null) return null;
        entity.Hindrances = request.Hindrances ?? entity.Hindrances;
        entity.NextDayPlan = request.NextDayPlan ?? entity.NextDayPlan;
        entity.Remarks = request.Remarks ?? entity.Remarks;
        entity.Status = request.Status;
        entity.IsActive = request.IsActive;
        entity.LastModifiedDate = DateTimeOffset.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
        return new Models.DailyProgressModel(entity.Id, entity.UniqueId, entity.ProjectId, entity.ReportDate, entity.Hindrances, entity.HindranceAudioUrl, entity.NextDayPlan, entity.Remarks, entity.TotalAmount, entity.Status, entity.IsActive, entity.CreatedBy, entity.CreatedDate, entity.LastModifiedBy, entity.LastModifiedDate);
    }

    public async Task<bool> DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await _db.Set<Himapp.Execution.Domain.Entities.DailyProgress>().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null) return false;
        entity.IsActive = false;
        entity.LastModifiedDate = DateTimeOffset.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }
}
