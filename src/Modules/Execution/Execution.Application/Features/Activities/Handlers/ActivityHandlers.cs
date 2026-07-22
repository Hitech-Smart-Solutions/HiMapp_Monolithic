using Himapp.Execution.Application.Features.Activities.Commands;
using Himapp.Execution.Application.Features.Activities.Queries;
using Himapp.Execution.Domain.Entities;
using Himapp.Execution.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Himapp.Execution.Application.Features.Activities.Handlers;

internal sealed class CreateActivityCommandHandler : IRequestHandler<CreateActivityCommand, ActivityDto>
{
    private readonly ExecutionDbContext _db;
    public CreateActivityCommandHandler(ExecutionDbContext db) => _db = db;

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

        _db.Activities.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);

        return new ActivityDto(entity.ID, request.CompanyID, request.ActivityName, request.UOMID, request.CreateBy, request.LastModifiedBy);
    }
}

internal sealed class UpdateActivityCommandHandler : IRequestHandler<UpdateActivityCommand, ActivityDto?>
{
    private readonly ExecutionDbContext _db;
    public UpdateActivityCommandHandler(ExecutionDbContext db) => _db = db;

    public async Task<ActivityDto?> Handle(UpdateActivityCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.Activities.FirstOrDefaultAsync(a => a.ID == request.Id, cancellationToken);
        if (entity is null) return null;

        entity.ActivityName = request.ActivityName;
        entity.UOMID = request.UOMID;
        entity.IsActive = true;
        entity.LastModifiedBy = request.LastModifiedBy;
        entity.LastModifiedDate = DateTimeOffset.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);

        return new ActivityDto(entity.ID, entity.CompanyID, entity.ActivityName, entity.UOMID, entity.CreatedBy, entity.LastModifiedBy);
    }
}

internal sealed class DeleteActivityCommandHandler : IRequestHandler<DeleteActivityCommand, bool>
{
    private readonly ExecutionDbContext _db;
    public DeleteActivityCommandHandler(ExecutionDbContext db) => _db = db;

    public async Task<bool> Handle(DeleteActivityCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.Activities.FirstOrDefaultAsync(a => a.ID == request.Id, cancellationToken);
        if (entity is null) return false;

        var pas = _db.ProjectActivities.Where(x => x.ActivityId == request.Id);
        _db.ProjectActivities.RemoveRange(pas);
        _db.Activities.Remove(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }
}

internal sealed class GetAllActivitiesQueryHandler : IRequestHandler<GetAllActivitiesQuery, IReadOnlyCollection<ActivityDto>>
{
    private readonly ExecutionDbContext _db;
    public GetAllActivitiesQueryHandler(ExecutionDbContext db) => _db = db;

    public async Task<IReadOnlyCollection<ActivityDto>> Handle(GetAllActivitiesQuery request, CancellationToken cancellationToken)
    {
        var activities = _db.Activities.AsNoTracking();
        var projectActivities = _db.ProjectActivities.AsNoTracking();

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
}

internal sealed class GetActivityByIdQueryHandler : IRequestHandler<GetActivityByIdQuery, ActivityDto?>
{
    private readonly ExecutionDbContext _db;
    public GetActivityByIdQueryHandler(ExecutionDbContext db) => _db = db;

    public async Task<ActivityDto?> Handle(GetActivityByIdQuery request, CancellationToken cancellationToken)
    {
        var activities = _db.Activities.AsNoTracking();
        var projectActivities = _db.ProjectActivities.AsNoTracking();

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

