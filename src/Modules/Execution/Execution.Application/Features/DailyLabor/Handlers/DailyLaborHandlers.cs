using Himapp.Execution.Application.Features.DailyLabor.Commands;
using Himapp.Execution.Application.Features.DailyLabor.Models;
using Himapp.Execution.Application.Features.DailyLabor.Queries;
using Himapp.Execution.Domain.Entities;
using Himapp.Execution.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Himapp.Execution.Application.Features.DailyLabor.Handlers;

internal sealed class GetAllDailyLaborsQueryHandler : IRequestHandler<GetAllDailyLaborsQuery, IReadOnlyCollection<DailyLaborModel>>
{
    private readonly ExecutionDbContext _db;
    public GetAllDailyLaborsQueryHandler(ExecutionDbContext db) => _db = db;

    public async Task<IReadOnlyCollection<DailyLaborModel>> Handle(GetAllDailyLaborsQuery request, CancellationToken cancellationToken)
    {
        return await _db.DailyLabors
            .AsNoTracking()
            .Select(d => new DailyLaborModel(d.ID, d.UniqueID, d.ProjectID, d.SlipDate, d.Remarks, d.StateID, d.IsActive, d.CreatedBy, d.CreatedDate, d.LastModifiedBy, d.LastModifiedDate))
            .ToArrayAsync(cancellationToken);
    }
}

internal sealed class GetDailyLaborByIdQueryHandler : IRequestHandler<GetDailyLaborByIdQuery, DailyLaborModel?>
{
    private readonly ExecutionDbContext _db;
    public GetDailyLaborByIdQueryHandler(ExecutionDbContext db) => _db = db;

    public async Task<DailyLaborModel?> Handle(GetDailyLaborByIdQuery request, CancellationToken cancellationToken)
    {
        var d = await _db.DailyLabors.AsNoTracking().FirstOrDefaultAsync(x => x.ID == request.Id, cancellationToken);
        if (d is null) return null;
        return new DailyLaborModel(d.ID, d.UniqueID, d.ProjectID, d.SlipDate, d.Remarks, d.StateID, d.IsActive, d.CreatedBy, d.CreatedDate, d.LastModifiedBy, d.LastModifiedDate);
    }
}

internal sealed class CreateDailyLaborCommandHandler : IRequestHandler<CreateDailyLaborCommand, DailyLaborModel>
{
    private readonly ExecutionDbContext _db;
    public CreateDailyLaborCommandHandler(ExecutionDbContext db) => _db = db;

    public async Task<DailyLaborModel> Handle(CreateDailyLaborCommand request, CancellationToken cancellationToken)
    {
        var r = request.Request;
        var entity = new Himapp.Execution.Domain.Entities.DailyDepartmentalLabourSlip
        {
            UniqueID = Guid.NewGuid(),
            ProjectID = r.ProjectID,
            SlipDate = r.SlipDate,
            Remarks = r.Remarks,
            StateID = 1,
            IsActive = true,
            CreatedBy = 1,
            CreatedDate = DateTime.UtcNow,
            LastModifiedBy = 1,
            LastModifiedDate = DateTime.UtcNow
        };

        _db.DailyLabors.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);

        return new DailyLaborModel(entity.ID, entity.UniqueID, entity.ProjectID, entity.SlipDate, entity.Remarks, entity.StateID, entity.IsActive, entity.CreatedBy, entity.CreatedDate, entity.LastModifiedBy, entity.LastModifiedDate);
    }
}

internal sealed class UpdateDailyLaborCommandHandler : IRequestHandler<UpdateDailyLaborCommand, DailyLaborModel>
{
    private readonly ExecutionDbContext _db;
    public UpdateDailyLaborCommandHandler(ExecutionDbContext db) => _db = db;

    public async Task<DailyLaborModel> Handle(UpdateDailyLaborCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.DailyLabors.FirstOrDefaultAsync(x => x.ID == request.Id, cancellationToken);
        if (entity is null) return null!;

        entity.Remarks = request.Request.Remarks ?? entity.Remarks;
        entity.StateID = request.Request.StateID;
        entity.IsActive = request.Request.IsActive;
        entity.LastModifiedDate = DateTime.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);

        return new DailyLaborModel(entity.ID, entity.UniqueID, entity.ProjectID, entity.SlipDate, entity.Remarks, entity.StateID, entity.IsActive, entity.CreatedBy, entity.CreatedDate, entity.LastModifiedBy, entity.LastModifiedDate);
    }
}

internal sealed class DeleteDailyLaborCommandHandler : IRequestHandler<DeleteDailyLaborCommand, bool>
{
    private readonly ExecutionDbContext _db;
    public DeleteDailyLaborCommandHandler(ExecutionDbContext db) => _db = db;

    public async Task<bool> Handle(DeleteDailyLaborCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.DailyLabors.FirstOrDefaultAsync(x => x.ID == request.Id, cancellationToken);
        if (entity is null) return false;

        entity.IsActive = false;
        entity.LastModifiedDate = DateTime.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }
}

