using Himapp.Execution.Application.Features.DailyDepartmentalLabourSlip.Models;
using Himapp.Execution.Application.Features.DailyDepartmentalLabourSlip.Commands;
using Himapp.Execution.Application.Features.DailyDepartmentalLabourSlip.Queries;
using Himapp.Execution.Application.Features.DailyDepartmentalLabourSlip.Models;
using Himapp.Execution.Application.Features.DailyDepartmentalLabourSlip.Commands;
using Himapp.Execution.Application.Features.DailyDepartmentalLabourSlip.Queries;
using Himapp.Execution.Domain.Entities;
using Himapp.Execution.Contracts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Himapp.Execution.Application.Features.DailyDepartmentalLabourSlip.Handlers;

internal sealed class DailyDepartmentalLabourSlipHandlers :
    IRequestHandler<GetAllDailyDepartmentalLabourSlipsQuery, IEnumerable<DailyDepartmentalLabourSlipDto>>,
    IRequestHandler<GetDailyDepartmentalLabourSlipByIdQuery, DailyDepartmentalLabourSlipDto?>,
    IRequestHandler<CreateDailyDepartmentalLabourSlipCommand, DailyDepartmentalLabourSlipDto>,
    IRequestHandler<UpdateDailyDepartmentalLabourSlipCommand, DailyDepartmentalLabourSlipDto?>,
    IRequestHandler<DeleteDailyDepartmentalLabourSlipCommand, bool>
{
    private readonly IExecutionDbContext _db;
    public DailyDepartmentalLabourSlipHandlers(IExecutionDbContext db) => _db = db;

    public async Task<IEnumerable<DailyDepartmentalLabourSlipDto>> Handle(GetAllDailyDepartmentalLabourSlipsQuery request, CancellationToken cancellationToken)
    {
        return await _db.Set<Himapp.Execution.Domain.Entities.DailyDepartmentalLabourSlip>()
            .AsNoTracking()
            .Where(d => d.IsActive)
            .Select(d => new DailyDepartmentalLabourSlipDto
            {
                Id = d.ID,
                ProgramId = d.ProjectID ?? 0
            })
            .ToArrayAsync(cancellationToken);
    }

    public async Task<DailyDepartmentalLabourSlipDto?> Handle(GetDailyDepartmentalLabourSlipByIdQuery request, CancellationToken cancellationToken)
    {
        var d = await _db.Set<Himapp.Execution.Domain.Entities.DailyDepartmentalLabourSlip>().AsNoTracking().FirstOrDefaultAsync(x => x.ID == request.Id && x.IsActive, cancellationToken);
        if (d is null) return null;
        return new DailyDepartmentalLabourSlipDto { Id = d.ID, ProgramId = d.ProjectID ?? 0 };
    }

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

        _db.Set<Himapp.Execution.Domain.Entities.DailyDepartmentalLabourSlip>().Add(entity);
        await _db.SaveChangesAsync(cancellationToken);

        return new DailyDepartmentalLabourSlipDto { Id = entity.ID, ProgramId = entity.ProjectID ?? 0 };
    }

    public async Task<DailyDepartmentalLabourSlipDto?> Handle(UpdateDailyDepartmentalLabourSlipCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.Set<Himapp.Execution.Domain.Entities.DailyDepartmentalLabourSlip>().FirstOrDefaultAsync(x => x.ID == request.Id && x.IsActive, cancellationToken);
        if (entity is null) return null;

        var r = request.Request;

        entity.ProjectID = r.ProjectId;
        entity.SlipDate = r.SlipDate?.UtcDateTime ?? entity.SlipDate;
        entity.Remarks = r.Remarks ?? entity.Remarks;
        entity.LastModifiedDate = DateTime.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);

        return new DailyDepartmentalLabourSlipDto { Id = entity.ID, ProgramId = entity.ProjectID ?? 0 };
    }

    public async Task<bool> Handle(DeleteDailyDepartmentalLabourSlipCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.Set<Himapp.Execution.Domain.Entities.DailyDepartmentalLabourSlip>().FirstOrDefaultAsync(x => x.ID == request.Id && x.IsActive, cancellationToken);
        if (entity is null) return false;

        entity.IsActive = false;
        entity.LastModifiedDate = DateTime.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }
}
