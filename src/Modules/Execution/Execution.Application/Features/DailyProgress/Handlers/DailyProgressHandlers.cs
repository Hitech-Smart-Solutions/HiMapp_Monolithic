using Himapp.Execution.Application.Features.DailyProgress.Models;
using Himapp.Execution.Application.Features.DailyProgress.Commands;
using Himapp.Execution.Application.Features.DailyProgress.Queries;
using Himapp.Execution.Domain.Entities;
using Himapp.Execution.Contracts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Himapp.Execution.Application.Features.DailyProgress.Handlers;

internal sealed class DailyProgressHandlers :
    IRequestHandler<GetAllDailyProgressQuery, IReadOnlyCollection<DailyProgressModel>>,
    IRequestHandler<GetDailyProgressByIdQuery, DailyProgressModel?>,
    IRequestHandler<CreateDailyProgressCommand, DailyProgressModel>,
    IRequestHandler<UpdateDailyProgressCommand, DailyProgressModel?>,
    IRequestHandler<DeleteDailyProgressCommand, bool>
{
    private readonly IExecutionDbContext _db;
    public DailyProgressHandlers(IExecutionDbContext db) => _db = db;

    public async Task<IReadOnlyCollection<DailyProgressModel>> Handle(GetAllDailyProgressQuery request, CancellationToken cancellationToken)
    {
        return await _db.Set<Himapp.Execution.Domain.Entities.DailyProgress>()
            .AsNoTracking()
            .Select(d => new DailyProgressModel(d.ID, d.UniqueID, d.ProjectID, d.ReportDate, d.Hindrances, d.HindranceAudioUrl, d.NextDayPlan, d.Remarks, d.TotalAmount, d.Status, d.IsActive, d.CreatedBy, d.CreatedDate, d.LastModifiedBy, d.LastModifiedDate))
            .ToArrayAsync(cancellationToken);
    }

    public async Task<DailyProgressModel?> Handle(GetDailyProgressByIdQuery request, CancellationToken cancellationToken)
    {
        var d = await _db.Set<Himapp.Execution.Domain.Entities.DailyProgress>().AsNoTracking().FirstOrDefaultAsync(x => x.ID == request.Id, cancellationToken);
        if (d is null) return null;
        return new DailyProgressModel(d.ID, d.UniqueID, d.ProjectID, d.ReportDate, d.Hindrances, d.HindranceAudioUrl, d.NextDayPlan, d.Remarks, d.TotalAmount, d.Status, d.IsActive, d.CreatedBy, d.CreatedDate, d.LastModifiedBy, d.LastModifiedDate);
    }

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

        _db.Set<Himapp.Execution.Domain.Entities.DailyProgress>().Add(entity);
        await _db.SaveChangesAsync(cancellationToken);

        return new DailyProgressModel(entity.ID, entity.UniqueID, entity.ProjectID, entity.ReportDate, entity.Hindrances, entity.HindranceAudioUrl, entity.NextDayPlan, entity.Remarks, entity.TotalAmount, entity.Status, entity.IsActive, entity.CreatedBy, entity.CreatedDate, entity.LastModifiedBy, entity.LastModifiedDate);
    }

    public async Task<DailyProgressModel?> Handle(UpdateDailyProgressCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.Set<Himapp.Execution.Domain.Entities.DailyProgress>().FirstOrDefaultAsync(x => x.ID == request.Id, cancellationToken);
        if (entity is null) return null;

        entity.Hindrances = request.Request.Hindrances ?? entity.Hindrances;
        entity.NextDayPlan = request.Request.NextDayPlan ?? entity.NextDayPlan;
        entity.Remarks = request.Request.Remarks ?? entity.Remarks;
        entity.Status = request.Request.Status;
        entity.IsActive = request.Request.IsActive;
        entity.LastModifiedDate = DateTimeOffset.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);

        return new DailyProgressModel(entity.ID, entity.UniqueID, entity.ProjectID, entity.ReportDate, entity.Hindrances, entity.HindranceAudioUrl, entity.NextDayPlan, entity.Remarks, entity.TotalAmount, entity.Status, entity.IsActive, entity.CreatedBy, entity.CreatedDate, entity.LastModifiedBy, entity.LastModifiedDate);
    }

    public async Task<bool> Handle(DeleteDailyProgressCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.Set<Himapp.Execution.Domain.Entities.DailyProgress>().FirstOrDefaultAsync(x => x.ID == request.Id, cancellationToken);
        if (entity is null) return false;

        entity.IsActive = false;
        entity.LastModifiedDate = DateTimeOffset.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }
}

