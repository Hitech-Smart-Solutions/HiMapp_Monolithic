using Himapp.Execution.Application.Features.SiteDailyProgress.Models;
using Himapp.Execution.Application.Features.SiteDailyProgress.Commands;
using Himapp.Execution.Application.Features.SiteDailyProgress.Queries;
using Himapp.Execution.Domain.Entities;
using Himapp.Execution.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Himapp.Execution.Application.Features.SiteDailyProgress.Handlers;

internal sealed class GetAllSiteDailyProgressesQueryHandler : IRequestHandler<GetAllSiteDailyProgressesQuery, IEnumerable<SiteDailyProgressDto>>
{
    private readonly ExecutionDbContext _db;
    public GetAllSiteDailyProgressesQueryHandler(ExecutionDbContext db) => _db = db;
    public async Task<IEnumerable<SiteDailyProgressDto>> Handle(GetAllSiteDailyProgressesQuery request, CancellationToken cancellationToken)
    {
        return await _db.SiteDailyProgresses
            .AsNoTracking()
            .Where(d => d.IsActive)
            .Select(d => new SiteDailyProgressDto { Id = d.Id, ProgramId = d.ProjectId })
            .ToArrayAsync(cancellationToken);
    }
}

internal sealed class GetSiteDailyProgressByIdQueryHandler : IRequestHandler<GetSiteDailyProgressByIdQuery, SiteDailyProgressDto?>
{
    private readonly ExecutionDbContext _db;
    public GetSiteDailyProgressByIdQueryHandler(ExecutionDbContext db) => _db = db;

    public async Task<SiteDailyProgressDto?> Handle(GetSiteDailyProgressByIdQuery request, CancellationToken cancellationToken)
    {
        var d = await _db.SiteDailyProgresses.AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.Id && x.IsActive, cancellationToken);
        if (d is null) return null;
        return new SiteDailyProgressDto { Id = d.Id, ProgramId = d.ProjectId };
    }
}

internal sealed class CreateSiteDailyProgressCommandHandler : IRequestHandler<CreateSiteDailyProgressCommand, SiteDailyProgressDto>
{
    private readonly ExecutionDbContext _db;
    public CreateSiteDailyProgressCommandHandler(ExecutionDbContext db) => _db = db;

    public async Task<SiteDailyProgressDto> Handle(CreateSiteDailyProgressCommand request, CancellationToken cancellationToken)
    {
        var r = request.Request;

        var entity = new Himapp.Execution.Domain.Entities.SiteDailyProgress
        {
            UniqueId = Guid.NewGuid(),
            ProjectId = r.ProjectId,
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

        _db.SiteDailyProgresses.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);

        return new SiteDailyProgressDto { Id = entity.Id, ProgramId = entity.ProjectId };
    }
}

internal sealed class UpdateSiteDailyProgressCommandHandler : IRequestHandler<UpdateSiteDailyProgressCommand, SiteDailyProgressDto?>
{
    private readonly ExecutionDbContext _db;
    public UpdateSiteDailyProgressCommandHandler(ExecutionDbContext db) => _db = db;

    public async Task<SiteDailyProgressDto?> Handle(UpdateSiteDailyProgressCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.SiteDailyProgresses.FirstOrDefaultAsync(x => x.Id == request.Id && x.IsActive, cancellationToken);
        if (entity is null) return null;

        var r = request.Request;

        entity.ProjectId = r.ProjectId;
        entity.ReportDate = r.ReportDate.HasValue ? DateOnly.FromDateTime(r.ReportDate.Value.UtcDateTime) : entity.ReportDate;
        entity.Remarks = r.Remarks ?? entity.Remarks;
        entity.LastModifiedDate = DateTimeOffset.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);

        return new SiteDailyProgressDto { Id = entity.Id, ProgramId = entity.ProjectId };
    }
}

internal sealed class DeleteSiteDailyProgressCommandHandler : IRequestHandler<DeleteSiteDailyProgressCommand, bool>
{
    private readonly ExecutionDbContext _db;
    public DeleteSiteDailyProgressCommandHandler(ExecutionDbContext db) => _db = db;

    public async Task<bool> Handle(DeleteSiteDailyProgressCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.SiteDailyProgresses.FirstOrDefaultAsync(x => x.Id == request.Id && x.IsActive, cancellationToken);
        if (entity is null) return false;

        entity.IsActive = false;
        entity.LastModifiedDate = DateTimeOffset.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }
}
