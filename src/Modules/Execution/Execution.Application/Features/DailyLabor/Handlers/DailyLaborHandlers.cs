using Himapp.Execution.Application.Features.DailyLabor.Commands;
using Himapp.Execution.Application.Features.DailyLabor.Models;
using Himapp.Execution.Application.Features.DailyLabor.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Himapp.Execution.Application.Features.DailyLabor.Handlers;

internal sealed class GetAllDailyLaborsQueryHandler : IRequestHandler<GetAllDailyLaborsQuery, IReadOnlyCollection<DailyLaborModel>>
{
    private readonly DbContext _db;
    public GetAllDailyLaborsQueryHandler(DbContext db) => _db = db;
    public Task<IReadOnlyCollection<DailyLaborModel>> Handle(GetAllDailyLaborsQuery request, CancellationToken cancellationToken) =>
        _db.Set<Himapp.Execution.Domain.Entities.DailyLabor>()
           .AsNoTracking()
           .Select(d => new DailyLaborModel(d.Id, d.UniqueId, d.ProjectId, d.ReportDate, d.Remarks, d.Status, d.IsActive, d.CreatedBy, d.CreatedDate, d.LastModifiedBy, d.LastModifiedDate))
           .ToArrayAsync(cancellationToken)
           .ContinueWith(t => (IReadOnlyCollection<DailyLaborModel>)t.Result, cancellationToken);
}

internal sealed class GetDailyLaborByIdQueryHandler : IRequestHandler<GetDailyLaborByIdQuery, DailyLaborModel?>
{
    private readonly DbContext _db;
    public GetDailyLaborByIdQueryHandler(DbContext db) => _db = db;
    public async Task<DailyLaborModel?> Handle(GetDailyLaborByIdQuery request, CancellationToken cancellationToken)
    {
        var d = await _db.Set<Himapp.Execution.Domain.Entities.DailyLabor>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (d is null) return null;
        return new DailyLaborModel(d.Id, d.UniqueId, d.ProjectId, d.ReportDate, d.Remarks, d.Status, d.IsActive, d.CreatedBy, d.CreatedDate, d.LastModifiedBy, d.LastModifiedDate);
    }
}

internal sealed class CreateDailyLaborCommandHandler : IRequestHandler<CreateDailyLaborCommand, DailyLaborModel>
{
    private readonly DbContext _db;
    public CreateDailyLaborCommandHandler(DbContext db) => _db = db;
    public async Task<DailyLaborModel> Handle(CreateDailyLaborCommand request, CancellationToken cancellationToken)
    {
        var r = request.Request;
        var entity = new Himapp.Execution.Domain.Entities.DailyLabor
        {
            UniqueId = Guid.NewGuid(),
            ProjectId = r.ProjectId,
            ReportDate = r.ReportDate,
            Remarks = r.Remarks,
            Status = "DRAFT",
            IsActive = true,
            CreatedBy = null,
            CreatedDate = DateTimeOffset.UtcNow,
            LastModifiedBy = null,
            LastModifiedDate = DateTimeOffset.UtcNow
        };
        _db.Set<Himapp.Execution.Domain.Entities.DailyLabor>().Add(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return new DailyLaborModel(entity.Id, entity.UniqueId, entity.ProjectId, entity.ReportDate, entity.Remarks, entity.Status, entity.IsActive, entity.CreatedBy, entity.CreatedDate, entity.LastModifiedBy, entity.LastModifiedDate);
    }
}

internal sealed class UpdateDailyLaborCommandHandler : IRequestHandler<UpdateDailyLaborCommand, DailyLaborModel>
{
    private readonly DbContext _db;
    public UpdateDailyLaborCommandHandler(DbContext db) => _db = db;
    public async Task<DailyLaborModel> Handle(UpdateDailyLaborCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.Set<Himapp.Execution.Domain.Entities.DailyLabor>().FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (entity is null) return null;
        entity.Remarks = request.Request.Remarks ?? entity.Remarks;
        entity.Status = request.Request.Status;
        entity.IsActive = request.Request.IsActive;
        entity.LastModifiedDate = DateTimeOffset.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
        return new DailyLaborModel(entity.Id, entity.UniqueId, entity.ProjectId, entity.ReportDate, entity.Remarks, entity.Status, entity.IsActive, entity.CreatedBy, entity.CreatedDate, entity.LastModifiedBy, entity.LastModifiedDate);
    }
}

internal sealed class DeleteDailyLaborCommandHandler : IRequestHandler<DeleteDailyLaborCommand, bool>
{
    private readonly DbContext _db;
    public DeleteDailyLaborCommandHandler(DbContext db) => _db = db;
    public async Task<bool> Handle(DeleteDailyLaborCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.Set<Himapp.Execution.Domain.Entities.DailyLabor>().FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (entity is null) return false;
        entity.IsActive = false;
        entity.LastModifiedDate = DateTimeOffset.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }
}
