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
            UniqueId = Guid.NewGuid(),
            CompanyId = 0,
            Name = request.ActivityCode,
            DefaultUom = "SQM",
            IsActive = true,
            CreatedBy = null,
            CreatedDate = DateTimeOffset.UtcNow,
            LastModifiedBy = null,
            LastModifiedDate = DateTimeOffset.UtcNow
        };

        _db.Activities.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);

        if (request.ProjectId > 0)
        {
            var pa = new ProjectActivity
            {
                UniqueId = Guid.NewGuid(),
                ProjectId = request.ProjectId,
                ActivityId = entity.Id,
                Enabled = true,
                IsActive = true,
                CreatedBy = null,
                CreatedDate = DateTimeOffset.UtcNow,
                LastModifiedBy = null,
                LastModifiedDate = DateTimeOffset.UtcNow
            };

            _db.ProjectActivities.Add(pa);
            await _db.SaveChangesAsync(cancellationToken);
        }

        return new ActivityDto(entity.Id, request.ProjectId, request.ActivityCode, request.Description, request.ProgressPercent, request.WorkDate);
    }
}

internal sealed class UpdateActivityCommandHandler : IRequestHandler<UpdateActivityCommand, ActivityDto?>
{
    private readonly ExecutionDbContext _db;
    public UpdateActivityCommandHandler(ExecutionDbContext db) => _db = db;

    public async Task<ActivityDto?> Handle(UpdateActivityCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.Activities.FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);
        if (entity is null) return null;

        entity.Name = request.ActivityCode;
        entity.DefaultUom = "SQM";
        entity.IsActive = true;
        entity.LastModifiedDate = DateTimeOffset.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);

        if (request.ProjectId > 0)
        {
            var pa = await _db.ProjectActivities.FirstOrDefaultAsync(x => x.ActivityId == entity.Id, cancellationToken);
            if (pa is null)
            {
                pa = new ProjectActivity
                {
                    UniqueId = Guid.NewGuid(),
                    ProjectId = request.ProjectId,
                    ActivityId = entity.Id,
                    Enabled = true,
                    IsActive = true,
                    CreatedBy = null,
                    CreatedDate = DateTimeOffset.UtcNow,
                    LastModifiedBy = null,
                    LastModifiedDate = DateTimeOffset.UtcNow
                };
                _db.ProjectActivities.Add(pa);
            }
            else
            {
                pa.ProjectId = request.ProjectId;
                pa.LastModifiedDate = DateTimeOffset.UtcNow;
            }

            await _db.SaveChangesAsync(cancellationToken);
        }

        return new ActivityDto(request.Id, request.ProjectId, request.ActivityCode, request.Description, request.ProgressPercent, request.WorkDate);
    }
}

internal sealed class DeleteActivityCommandHandler : IRequestHandler<DeleteActivityCommand, bool>
{
    private readonly ExecutionDbContext _db;
    public DeleteActivityCommandHandler(ExecutionDbContext db) => _db = db;

    public async Task<bool> Handle(DeleteActivityCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.Activities.FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);
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
                           join pa in projectActivities on a.Id equals pa.ActivityId into pas
                           from pa in pas.DefaultIfEmpty()
                           orderby a.Id
                           select new ActivityDto(
                               a.Id,
                               pa == null ? 0 : pa.ProjectId,
                               a.Name,
                               a.Name,
                               0m,
                               default))
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
                         join pa in projectActivities on a.Id equals pa.ActivityId into pas
                         from pa in pas.DefaultIfEmpty()
                         where a.Id == request.Id
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
}

