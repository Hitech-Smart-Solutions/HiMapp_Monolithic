using Himapp.Execution.Application.Features.Activities;
using Himapp.Execution.Domain.Entities;
using Himapp.SharedKernel.Abstractions;
using Microsoft.EntityFrameworkCore;
// removed unused usings

namespace Himapp.Execution.Application.Features.Activities;

internal sealed class EfActivityRepository : IActivityRepository
{
    private readonly DbContext _db;

    public EfActivityRepository(DbContext db) => _db = db;

    public async Task<IReadOnlyCollection<ActivityDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        // Join Activity with ProjectActivity (if any) to obtain a ProjectId for the DTO.
        var activities = _db.Set<Activity>().AsNoTracking();
        var projectActivities = _db.Set<ProjectActivity>().AsNoTracking();

        var items = await (from a in activities
                           join pa in projectActivities on a.Id equals pa.ActivityId into pas
                           from pa in pas.DefaultIfEmpty()
                           orderby a.Id
                           select new ActivityDto(
                               a.Id,
                               pa == null ? 0 : pa.ProjectId,
                               a.Name, // use Name as ActivityCode -- domain doesn't have a separate code field
                               a.Name, // use Name also as Description until description column exists
                               0m, // progress not stored on Activity entity
                               default))
                          .ToArrayAsync(cancellationToken);

        return items;
    }

    public async Task<ActivityDto?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var activities = _db.Set<Activity>().AsNoTracking();
        var projectActivities = _db.Set<ProjectActivity>().AsNoTracking();

        var dto = await (from a in activities
                         join pa in projectActivities on a.Id equals pa.ActivityId into pas
                         from pa in pas.DefaultIfEmpty()
                         where a.Id == id
                         select new ActivityDto(
                             a.Id,
                             pa == null ? 0 : pa.ProjectId,
                             a.Name,
                             a.Name,
                             0m,
                             default))
                        .FirstOrDefaultAsync(cancellationToken);

        return dto;
    }

    public async Task<ActivityDto> AddAsync(ActivityDto activity, CancellationToken cancellationToken = default)
    {
        var entity = new Activity
        {
            UniqueId = Guid.NewGuid(),
            CompanyId = 0,
            Name = activity.ActivityCode,
            DefaultUom = "SQM",
            IsActive = true,
            CreatedBy = null,
            CreatedDate = DateTimeOffset.UtcNow,
            LastModifiedBy = null,
            LastModifiedDate = DateTimeOffset.UtcNow
        };

        _db.Set<Activity>().Add(entity);
        await _db.SaveChangesAsync(cancellationToken);

        // If a ProjectId was provided, create linking ProjectActivity record.
        if (activity.ProjectId > 0)
        {
            var pa = new ProjectActivity
            {
                UniqueId = Guid.NewGuid(),
                ProjectId = activity.ProjectId,
                ActivityId = entity.Id,
                Enabled = true,
                IsActive = true,
                CreatedBy = null,
                CreatedDate = DateTimeOffset.UtcNow,
                LastModifiedBy = null,
                LastModifiedDate = DateTimeOffset.UtcNow
            };

            _db.Set<ProjectActivity>().Add(pa);
            await _db.SaveChangesAsync(cancellationToken);
        }

        return new ActivityDto(entity.Id, activity.ProjectId, activity.ActivityCode, activity.Description, activity.ProgressPercent, activity.WorkDate);
    }

    public async Task<ActivityDto?> UpdateAsync(ActivityDto activity, CancellationToken cancellationToken = default)
    {
        var entity = await _db.Set<Activity>().FirstOrDefaultAsync(a => a.Id == activity.Id, cancellationToken);
        if (entity is null) return null;

        entity.Name = activity.ActivityCode;
        entity.DefaultUom = "SQM";
        entity.IsActive = true;
        entity.LastModifiedDate = DateTimeOffset.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);

        // Update or create ProjectActivity linking if ProjectId provided
        if (activity.ProjectId > 0)
        {
            var pa = await _db.Set<ProjectActivity>().FirstOrDefaultAsync(x => x.ActivityId == entity.Id, cancellationToken);
            if (pa is null)
            {
                pa = new ProjectActivity
                {
                    UniqueId = Guid.NewGuid(),
                    ProjectId = activity.ProjectId,
                    ActivityId = entity.Id,
                    Enabled = true,
                    IsActive = true,
                    CreatedBy = null,
                    CreatedDate = DateTimeOffset.UtcNow,
                    LastModifiedBy = null,
                    LastModifiedDate = DateTimeOffset.UtcNow
                };
                _db.Set<ProjectActivity>().Add(pa);
            }
            else
            {
                pa.ProjectId = activity.ProjectId;
                pa.LastModifiedDate = DateTimeOffset.UtcNow;
            }

            await _db.SaveChangesAsync(cancellationToken);
        }

        return activity;
    }

    public async Task<bool> DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await _db.Set<Activity>().FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
        if (entity is null) return false;

        // Remove any project links first
        var pas = _db.Set<ProjectActivity>().Where(x => x.ActivityId == id);
        _db.Set<ProjectActivity>().RemoveRange(pas);

        _db.Set<Activity>().Remove(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }
}

