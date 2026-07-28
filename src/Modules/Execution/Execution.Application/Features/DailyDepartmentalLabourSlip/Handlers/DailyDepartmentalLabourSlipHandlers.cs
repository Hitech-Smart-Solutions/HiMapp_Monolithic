using Himapp.Execution.Application.Features.DailyDepartmentalLabourSlip.Models;
using Himapp.Execution.Application.Features.DailyDepartmentalLabourSlip.Commands;
using Himapp.Execution.Application.Features.DailyDepartmentalLabourSlip.Queries;
using Himapp.Execution.Domain.Entities;
using Himapp.Execution.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Himapp.Execution.Application.Features.DailyDepartmentalLabourSlip.Handlers;

internal sealed class GetAllDailyDepartmentalLabourSlipsQueryHandler : IRequestHandler<GetAllDailyDepartmentalLabourSlipsQuery, IEnumerable<DailyDepartmentalLabourSlipDto>>
{
    private readonly ExecutionDbContext _db;
    public GetAllDailyDepartmentalLabourSlipsQueryHandler(ExecutionDbContext db) => _db = db;
    public async Task<IEnumerable<DailyDepartmentalLabourSlipDto>> Handle(GetAllDailyDepartmentalLabourSlipsQuery request, CancellationToken cancellationToken)
    {
        return await _db.DailyDepartmentalLabourSlips
            .AsNoTracking()
            .Where(d => d.IsActive)
            .Select(d => new DailyDepartmentalLabourSlipDto
            {
                Id = d.ID,
                ProgramId = d.ProjectID ?? 0
            })
            .ToArrayAsync(cancellationToken);
    }
}

internal sealed class GetDailyDepartmentalLabourSlipByIdQueryHandler : IRequestHandler<GetDailyDepartmentalLabourSlipByIdQuery, DailyDepartmentalLabourSlipDto?>
{
    private readonly ExecutionDbContext _db;
    public GetDailyDepartmentalLabourSlipByIdQueryHandler(ExecutionDbContext db) => _db = db;

    public async Task<DailyDepartmentalLabourSlipDto?> Handle(GetDailyDepartmentalLabourSlipByIdQuery request, CancellationToken cancellationToken)
    {
        var d = await _db.DailyDepartmentalLabourSlips.AsNoTracking().FirstOrDefaultAsync(x => x.ID == request.Id && x.IsActive, cancellationToken);
        if (d is null) return null;
        return new DailyDepartmentalLabourSlipDto { Id = d.ID, ProgramId = d.ProjectID ?? 0 };
    }
}

internal sealed class CreateDailyDepartmentalLabourSlipCommandHandler : IRequestHandler<CreateDailyDepartmentalLabourSlipCommand, DailyDepartmentalLabourSlipDto>
{
    private readonly ExecutionDbContext _db;
    public CreateDailyDepartmentalLabourSlipCommandHandler(ExecutionDbContext db) => _db = db;

    public async Task<DailyDepartmentalLabourSlipDto> Handle(CreateDailyDepartmentalLabourSlipCommand request, CancellationToken cancellationToken)
    {
        var r = request.Request;

        var entity = new Himapp.Execution.Domain.Entities.DailyDepartmentalLabourSlip
        {
            UniqueID = Guid.NewGuid(),
            ProjectID = r.ProjectId,
            SlipDate = r.SlipDate?.UtcDateTime,
            Remarks = r.Remarks,
            IsActive = true,
            CreatedBy = 0,
            CreatedDate = DateTime.UtcNow,
            LastModifiedBy = 0,
            LastModifiedDate = DateTime.UtcNow
        };

        _db.DailyDepartmentalLabourSlips.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);

        return new DailyDepartmentalLabourSlipDto { Id = entity.ID, ProgramId = entity.ProjectID ?? 0 };
    }
}

internal sealed class UpdateDailyDepartmentalLabourSlipCommandHandler : IRequestHandler<UpdateDailyDepartmentalLabourSlipCommand, DailyDepartmentalLabourSlipDto?>
{
    private readonly ExecutionDbContext _db;
    public UpdateDailyDepartmentalLabourSlipCommandHandler(ExecutionDbContext db) => _db = db;

    public async Task<DailyDepartmentalLabourSlipDto?> Handle(UpdateDailyDepartmentalLabourSlipCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.DailyDepartmentalLabourSlips.FirstOrDefaultAsync(x => x.ID == request.Id && x.IsActive, cancellationToken);
        if (entity is null) return null;

        var r = request.Request;

        entity.ProjectID = r.ProjectId;
        entity.SlipDate = r.SlipDate?.UtcDateTime ?? entity.SlipDate;
        entity.Remarks = r.Remarks ?? entity.Remarks;
        entity.LastModifiedDate = DateTime.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);

        return new DailyDepartmentalLabourSlipDto { Id = entity.ID, ProgramId = entity.ProjectID ?? 0 };
    }
}

internal sealed class DeleteDailyDepartmentalLabourSlipCommandHandler : IRequestHandler<DeleteDailyDepartmentalLabourSlipCommand, bool>
{
    private readonly ExecutionDbContext _db;
    public DeleteDailyDepartmentalLabourSlipCommandHandler(ExecutionDbContext db) => _db = db;

    public async Task<bool> Handle(DeleteDailyDepartmentalLabourSlipCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.DailyDepartmentalLabourSlips.FirstOrDefaultAsync(x => x.ID == request.Id && x.IsActive, cancellationToken);
        if (entity is null) return false;

        entity.IsActive = false;
        entity.LastModifiedDate = DateTime.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }
}
