using Himapp.Execution.Application.Features.Activities.Commands;
using Himapp.Execution.Application.Features.Activities.Queries;
using Himapp.Execution.Domain.Entities;
using Himapp.Execution.Contracts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Himapp.Execution.Application.Features.Activities.Handlers;

internal sealed class ActivityHandlers :
    IRequestHandler<CreateActivityCommand, ActivityDto>,
    IRequestHandler<UpdateActivityCommand, ActivityDto?>,
    IRequestHandler<DeleteActivityCommand, bool>,
    IRequestHandler<GetAllActivitiesQuery, IReadOnlyCollection<ActivityDto>>,
    IRequestHandler<GetActivityByIdQuery, ActivityDto?>
{
    private readonly IExecutionDbContext _db;
    public ActivityHandlers(IExecutionDbContext db) => _db = db;

    public async Task<ActivityDto> Handle(CreateActivityCommand request, CancellationToken cancellationToken)
    {
        var entity = new Activity
        {
            UniqueID = Guid.NewGuid(),
            CompanyID = request.CompanyID,
            ActivityName = request.ActivityName,
            UOMID = request.UOMID,
            IsActive = true,
            CreatedBy = request.CreateBy,
            CreatedDate = DateTimeOffset.UtcNow,
            LastModifiedBy = request.LastModifiedBy,
            LastModifiedDate = DateTimeOffset.UtcNow
        };

        _db.Set<Activity>().Add(entity);
        await _db.SaveChangesAsync(cancellationToken);

        return new ActivityDto(entity.ID, request.CompanyID, request.ActivityName, request.UOMID, request.CreateBy, request.LastModifiedBy);
    }

    public async Task<ActivityDto?> Handle(UpdateActivityCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.Set<Activity>().FirstOrDefaultAsync(a => a.ID == request.Id, cancellationToken);
        if (entity is null) return null;

        entity.ActivityName = request.ActivityName;
        entity.UOMID = request.UOMID;
        entity.IsActive = true;
        entity.LastModifiedBy = request.LastModifiedBy;
        entity.LastModifiedDate = DateTimeOffset.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);

        return new ActivityDto(entity.ID, entity.CompanyID, entity.ActivityName, entity.UOMID, entity.CreatedBy, entity.LastModifiedBy);
    }

    public async Task<bool> Handle(DeleteActivityCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.Set<Activity>().FirstOrDefaultAsync(a => a.ID == request.Id, cancellationToken);
        if (entity is null) return false;

        var pas = _db.Set<ProjectActivity>().Where(x => x.ActivityID == request.Id);
        _db.Set<ProjectActivity>().RemoveRange(pas);
        _db.Set<Activity>().Remove(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<IReadOnlyCollection<ActivityDto>> Handle(GetAllActivitiesQuery request, CancellationToken cancellationToken)
    {
        var activities = _db.Set<Activity>().AsNoTracking();

        var items = await (from a in activities
                           orderby a.ID
                           select new ActivityDto(
                               a.ID,
                               a.CompanyID,
                               a.ActivityName,
                               a.UOMID,
                               a.CreatedBy,
                               a.LastModifiedBy))
                          .ToArrayAsync(cancellationToken);

        return items;
    }

    public async Task<ActivityDto?> Handle(GetActivityByIdQuery request, CancellationToken cancellationToken)
    {
        var activities = _db.Set<Activity>().AsNoTracking();

        var dto = await (from a in activities
                         where a.ID == request.Id
                         select new ActivityDto(
                             a.ID,
                             a.CompanyID,
                             a.ActivityName,
                             a.UOMID,
                             a.CreatedBy,
                             a.LastModifiedBy))
                        .FirstOrDefaultAsync(cancellationToken);

        return dto;
    }
}

