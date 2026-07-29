using Himapp.Execution.Application.Features.SiteDailyProgress.Models;
using Himapp.Execution.Application.Features.SiteDailyProgress.Commands;
using Himapp.Execution.Application.Features.SiteDailyProgress.Queries;
using Himapp.Execution.Domain.Entities;
using Himapp.Execution.Contracts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Himapp.Execution.Application.Features.SiteDailyProgress.Handlers;

internal sealed class SiteDailyProgressHandlers :
    IRequestHandler<GetAllSiteDailyProgressesQuery, IEnumerable<SiteDailyProgressDto>>,
    IRequestHandler<GetSiteDailyProgressByIdQuery, SiteDailyProgressDto?>,
    IRequestHandler<CreateSiteDailyProgressCommand, SiteDailyProgressDto>,
    IRequestHandler<UpdateSiteDailyProgressCommand, SiteDailyProgressDto?>,
    IRequestHandler<DeleteSiteDailyProgressCommand, bool>
{
    private readonly IExecutionDbContext _db;
    public SiteDailyProgressHandlers(IExecutionDbContext db) => _db = db;

    public async Task<IEnumerable<SiteDailyProgressDto>> Handle(GetAllSiteDailyProgressesQuery request, CancellationToken cancellationToken)
    {
        return await _db.Set<Himapp.Execution.Domain.Entities.SiteDailyProgress>()
            .AsNoTracking()
            .Where(d => d.IsActive)
            .Select(d => new SiteDailyProgressDto { Id = d.ID, ProgramId = d.ProjectID })
            .ToArrayAsync(cancellationToken);
    }

    public async Task<SiteDailyProgressDto?> Handle(GetSiteDailyProgressByIdQuery request, CancellationToken cancellationToken)
    {
        var d = await _db.Set<Himapp.Execution.Domain.Entities.SiteDailyProgress>().AsNoTracking().FirstOrDefaultAsync(x => x.ID == request.Id && x.IsActive, cancellationToken);
        if (d is null) return null;
        return new SiteDailyProgressDto { Id = d.ID, ProgramId = d.ProjectID };
    }

    public async Task<SiteDailyProgressDto> Handle(CreateSiteDailyProgressCommand request, CancellationToken cancellationToken)
    {
        var r = request.Request;

        var entity = new Himapp.Execution.Domain.Entities.SiteDailyProgress
        {
            UniqueID = Guid.NewGuid(),
            ProjectID = r.ProjectId,
            SectionID = 0,
            ReportDate = r.ReportDate.HasValue ? DateOnly.FromDateTime(r.ReportDate.Value.UtcDateTime) : DateOnly.FromDateTime(DateTime.UtcNow),
            Hindrances = r.Remarks,
            NextDayPlan = null,
            Remarks = r.Remarks,
            TotalAmount = 0m,
            IsActive = true,
            CreatedBy = 0,
            CreatedDate = DateTimeOffset.UtcNow,
            LastModifiedBy = 0,
            LastModifiedDate = DateTimeOffset.UtcNow
        };

        _db.Set<Himapp.Execution.Domain.Entities.SiteDailyProgress>().Add(entity);
        await _db.SaveChangesAsync(cancellationToken);

        return new SiteDailyProgressDto { Id = entity.ID, ProgramId = entity.ProjectID };
    }

    public async Task<SiteDailyProgressDto?> Handle(UpdateSiteDailyProgressCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.Set<Himapp.Execution.Domain.Entities.SiteDailyProgress>().FirstOrDefaultAsync(x => x.ID == request.Id && x.IsActive, cancellationToken);
        if (entity is null) return null;

        var r = request.Request;

        entity.ProjectID = r.ProjectId;
        entity.ReportDate = r.ReportDate.HasValue ? DateOnly.FromDateTime(r.ReportDate.Value.UtcDateTime) : entity.ReportDate;
        entity.Remarks = r.Remarks ?? entity.Remarks;
        entity.LastModifiedDate = DateTimeOffset.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);

        return new SiteDailyProgressDto { Id = entity.ID, ProgramId = entity.ProjectID };
    }

    public async Task<bool> Handle(DeleteSiteDailyProgressCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.Set<Himapp.Execution.Domain.Entities.SiteDailyProgress>().FirstOrDefaultAsync(x => x.ID == request.Id && x.IsActive, cancellationToken);
        if (entity is null) return false;

        entity.IsActive = false;
        entity.LastModifiedDate = DateTimeOffset.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }
}
