using Himapp.Execution.Application.Features.DailyProgress.Models;
using Himapp.Execution.Application.Features.DailyProgress.Commands;
using Himapp.Execution.Application.Features.DailyProgress.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Himapp.Execution.Application.Features.DailyProgress.Handlers;

internal sealed class GetAllDailyProgressQueryHandler : IRequestHandler<GetAllDailyProgressQuery, IReadOnlyCollection<DailyProgressModel>>
{
    private readonly DbContext _db;
    public GetAllDailyProgressQueryHandler(DbContext db) => _db = db;
    public Task<IReadOnlyCollection<DailyProgressModel>> Handle(GetAllDailyProgressQuery request, CancellationToken cancellationToken) =>
        _db.Set<Himapp.Execution.Domain.Entities.DailyProgress>()
           .AsNoTracking()
           .Select(d => new DailyProgressModel(d.Id, d.UniqueId, d.ProjectId, d.ReportDate, d.Hindrances, d.HindranceAudioUrl, d.NextDayPlan, d.Remarks, d.TotalAmount, d.Status, d.IsActive, d.CreatedBy, d.CreatedDate, d.LastModifiedBy, d.LastModifiedDate))
           .ToArrayAsync(cancellationToken)
           .ContinueWith(t => (IReadOnlyCollection<DailyProgressModel>)t.Result, cancellationToken);
}

internal sealed class GetDailyProgressByIdQueryHandler : IRequestHandler<GetDailyProgressByIdQuery, DailyProgressModel?>
{
    private readonly DbContext _db;
    public GetDailyProgressByIdQueryHandler(DbContext db) => _db = db;
    public async Task<DailyProgressModel?> Handle(GetDailyProgressByIdQuery request, CancellationToken cancellationToken)
    {
        var d = await _db.Set<Himapp.Execution.Domain.Entities.DailyProgress>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (d is null) return null;
        return new DailyProgressModel(d.Id, d.UniqueId, d.ProjectId, d.ReportDate, d.Hindrances, d.HindranceAudioUrl, d.NextDayPlan, d.Remarks, d.TotalAmount, d.Status, d.IsActive, d.CreatedBy, d.CreatedDate, d.LastModifiedBy, d.LastModifiedDate);
    }
}

internal sealed class CreateDailyProgressCommandHandler : IRequestHandler<CreateDailyProgressCommand, DailyProgressModel>
{
    private readonly DbContext _db;
    public CreateDailyProgressCommandHandler(DbContext db) => _db = db;
    public async Task<DailyProgressModel> Handle(CreateDailyProgressCommand request, CancellationToken cancellationToken)
    {
        var r = request.Request;
        var entity = new Himapp.Execution.Domain.Entities.DailyProgress
        {
            UniqueId = Guid.NewGuid(),
            ProjectId = r.ProjectId,
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
        return new DailyProgressModel(entity.Id, entity.UniqueId, entity.ProjectId, entity.ReportDate, entity.Hindrances, entity.HindranceAudioUrl, entity.NextDayPlan, entity.Remarks, entity.TotalAmount, entity.Status, entity.IsActive, entity.CreatedBy, entity.CreatedDate, entity.LastModifiedBy, entity.LastModifiedDate);
    }
}

internal sealed class UpdateDailyProgressCommandHandler : IRequestHandler<UpdateDailyProgressCommand, DailyProgressModel?>
{
    private readonly DbContext _db;
    public UpdateDailyProgressCommandHandler(DbContext db) => _db = db;
    public async Task<DailyProgressModel?> Handle(UpdateDailyProgressCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.Set<Himapp.Execution.Domain.Entities.DailyProgress>().FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (entity is null) return null;
        entity.Hindrances = request.Request.Hindrances ?? entity.Hindrances;
        entity.NextDayPlan = request.Request.NextDayPlan ?? entity.NextDayPlan;
        entity.Remarks = request.Request.Remarks ?? entity.Remarks;
        entity.Status = request.Request.Status;
        entity.IsActive = request.Request.IsActive;
        entity.LastModifiedDate = DateTimeOffset.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
        return new DailyProgressModel(entity.Id, entity.UniqueId, entity.ProjectId, entity.ReportDate, entity.Hindrances, entity.HindranceAudioUrl, entity.NextDayPlan, entity.Remarks, entity.TotalAmount, entity.Status, entity.IsActive, entity.CreatedBy, entity.CreatedDate, entity.LastModifiedBy, entity.LastModifiedDate);
    }
}

internal sealed class DeleteDailyProgressCommandHandler : IRequestHandler<DeleteDailyProgressCommand, bool>
{
    private readonly DbContext _db;
    public DeleteDailyProgressCommandHandler(DbContext db) => _db = db;
    public async Task<bool> Handle(DeleteDailyProgressCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.Set<Himapp.Execution.Domain.Entities.DailyProgress>().FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (entity is null) return false;
        entity.IsActive = false;
        entity.LastModifiedDate = DateTimeOffset.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }
}
