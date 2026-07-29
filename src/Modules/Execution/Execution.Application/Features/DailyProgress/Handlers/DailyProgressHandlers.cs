using Himapp.Execution.Application.Features.DailyProgress.Models;
using Himapp.Execution.Application.Features.DailyProgress.Commands;
using Himapp.Execution.Application.Features.DailyProgress.Queries;
using Himapp.Execution.Domain.Entities;
using Himapp.Execution.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Himapp.Execution.Application.Features.DailyProgress.Handlers;

internal sealed class GetAllDailyProgressQueryHandler : IRequestHandler<GetAllDailyProgressQuery, IReadOnlyCollection<DailyProgressModel>>
{
    private readonly ExecutionDbContext _db;
    public GetAllDailyProgressQueryHandler(ExecutionDbContext db) => _db = db;

    public async Task<IReadOnlyCollection<DailyProgressModel>> Handle(GetAllDailyProgressQuery request, CancellationToken cancellationToken)
    {
        return await _db.DailyProgresses
            .AsNoTracking()
            .Select(d => new DailyProgressModel((int)d.ID, d.UniqueID, (int)d.ProjectID, d.ReportDate, d.Hindrances, d.HindranceAudioUrl, d.NextDayPlan, d.Remarks, d.TotalAmount, d.Status, d.IsActive, (int?)d.CreatedBy, d.CreatedDate, (int?)d.LastModifiedBy, d.LastModifiedDate))
            .ToArrayAsync(cancellationToken);
    }
}

internal sealed class GetDailyProgressByIdQueryHandler : IRequestHandler<GetDailyProgressByIdQuery, DailyProgressModel?>
{
    private readonly ExecutionDbContext _db;
    public GetDailyProgressByIdQueryHandler(ExecutionDbContext db) => _db = db;

    public async Task<DailyProgressModel?> Handle(GetDailyProgressByIdQuery request, CancellationToken cancellationToken)
    {
        var d = await _db.DailyProgresses.AsNoTracking().FirstOrDefaultAsync(x => x.ID == request.Id, cancellationToken);
        if (d is null) return null;
        return new DailyProgressModel((int)d.ID, d.UniqueID, (int)d.ProjectID, d.ReportDate, d.Hindrances, d.HindranceAudioUrl, d.NextDayPlan, d.Remarks, d.TotalAmount, d.Status, d.IsActive, (int?)d.CreatedBy, d.CreatedDate, (int?)d.LastModifiedBy, d.LastModifiedDate);
    }
}

internal sealed class CreateDailyProgressCommandHandler : IRequestHandler<CreateDailyProgressCommand, DailyProgressModel>
{
    private readonly ExecutionDbContext _db;
    public CreateDailyProgressCommandHandler(ExecutionDbContext db) => _db = db;

    public async Task<DailyProgressModel> Handle(CreateDailyProgressCommand request, CancellationToken cancellationToken)
    {
        var r = request.Request;
        var entity = new Himapp.Execution.Domain.Entities.DailyProgress
        {
            UniqueID = Guid.NewGuid(),
            ProjectID = r.ProjectId,
            ReportDate = r.ReportDate,
            Hindrances = r.Hindrances,
            NextDayPlan = r.NextDayPlan,
            Remarks = r.Remarks,
            TotalAmount = 0m,
            Status = "DRAFT",
            IsActive = true,
            CreatedBy = null,
            CreatedDate = DateTimeOffset.UtcNow,
            LastModifiedBy = null,
            LastModifiedDate = DateTimeOffset.UtcNow
        };

        _db.DailyProgresses.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);

        return new DailyProgressModel((int)entity.ID, entity.UniqueID, (int)entity.ProjectID, entity.ReportDate, entity.Hindrances, entity.HindranceAudioUrl, entity.NextDayPlan, entity.Remarks, entity.TotalAmount, entity.Status, entity.IsActive, (int?)entity.CreatedBy, entity.CreatedDate, (int?)entity.LastModifiedBy, entity.LastModifiedDate);
    }
}

internal sealed class UpdateDailyProgressCommandHandler : IRequestHandler<UpdateDailyProgressCommand, DailyProgressModel?>
{
    private readonly ExecutionDbContext _db;
    public UpdateDailyProgressCommandHandler(ExecutionDbContext db) => _db = db;

    public async Task<DailyProgressModel?> Handle(UpdateDailyProgressCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.DailyProgresses.FirstOrDefaultAsync(x => x.ID == request.Id, cancellationToken);
        if (entity is null) return null;

        entity.Hindrances = request.Request.Hindrances ?? entity.Hindrances;
        entity.NextDayPlan = request.Request.NextDayPlan ?? entity.NextDayPlan;
        entity.Remarks = request.Request.Remarks ?? entity.Remarks;
        entity.Status = request.Request.Status;
        entity.IsActive = request.Request.IsActive;
        entity.LastModifiedDate = DateTimeOffset.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);

        return new DailyProgressModel((int)entity.ID, entity.UniqueID, (int)entity.ProjectID, entity.ReportDate, entity.Hindrances, entity.HindranceAudioUrl, entity.NextDayPlan, entity.Remarks, entity.TotalAmount, entity.Status, entity.IsActive, (int?)entity.CreatedBy, entity.CreatedDate, (int?)entity.LastModifiedBy, entity.LastModifiedDate);
    }
}

internal sealed class DeleteDailyProgressCommandHandler : IRequestHandler<DeleteDailyProgressCommand, bool>
{
    private readonly ExecutionDbContext _db;
    public DeleteDailyProgressCommandHandler(ExecutionDbContext db) => _db = db;

    public async Task<bool> Handle(DeleteDailyProgressCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.DailyProgresses.FirstOrDefaultAsync(x => x.ID == request.Id, cancellationToken);
        if (entity is null) return false;

        entity.IsActive = false;
        entity.LastModifiedDate = DateTimeOffset.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }
}

