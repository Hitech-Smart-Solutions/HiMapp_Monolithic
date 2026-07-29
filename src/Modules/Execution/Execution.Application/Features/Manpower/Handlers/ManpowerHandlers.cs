using Himapp.Execution.Application.Features.Manpower.Models;
using Himapp.Execution.Application.Features.Manpower.Commands;
using Himapp.Execution.Application.Features.Manpower.Queries;
using Himapp.Execution.Domain.Entities;
using Himapp.Execution.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Himapp.Execution.Application.Features.Manpower.Handlers;

internal sealed class GetAllManpowersQueryHandler : IRequestHandler<GetAllManpowersQuery, IReadOnlyCollection<ManpowerModel>>
{
    private readonly ExecutionDbContext _db;
    public GetAllManpowersQueryHandler(ExecutionDbContext db) => _db = db;

    public async Task<IReadOnlyCollection<ManpowerModel>> Handle(GetAllManpowersQuery request, CancellationToken cancellationToken)
    {
        return await _db.Manpowers
            .AsNoTracking()
            .Select(m => new ManpowerModel(
                m.ID,
                m.UniqueID,
                m.ProjectID,
                m.SectionID,
                m.EntryDate,
                m.Remarks,
                m.StateID,
                m.IsActive,
                m.CreatedBy,
                m.CreatedDate,
                m.LastModifiedBy,
                m.LastModifiedDate,
                Array.Empty<ManpowerDetailModel>()))
            .ToArrayAsync(cancellationToken);
    }
}

internal sealed class GetManpowerByIdQueryHandler : IRequestHandler<GetManpowerByIdQuery, ManpowerModel?>
{
    private readonly ExecutionDbContext _db;
    public GetManpowerByIdQueryHandler(ExecutionDbContext db) => _db = db;

    public async Task<ManpowerModel?> Handle(GetManpowerByIdQuery request, CancellationToken cancellationToken)
    {
        var m = await _db.Manpowers
            .AsNoTracking()
            .Include(x => x.ManpowerDetail)
            .FirstOrDefaultAsync(x => x.ID == request.Id && x.IsActive, cancellationToken);
        if (m is null) return null;

        var details = m.ManpowerDetail?.Select(d => new ManpowerDetailModel(
            d.ID,
            d.UniqueID,
            d.ContractorID,
            d.ActivityID,
            d.SkilledCount,
            d.UnskilledCount,
            d.OtherCount,
            d.TotalCount)).ToArray() ?? Array.Empty<ManpowerDetailModel>();

        return new ManpowerModel(m.ID, m.UniqueID, m.ProjectID, m.SectionID, m.EntryDate, m.Remarks, m.StateID, m.IsActive, m.CreatedBy, m.CreatedDate, m.LastModifiedBy, m.LastModifiedDate, details);
    }
}

internal sealed class CreateManpowerCommandHandler : IRequestHandler<CreateManpowerCommand, ManpowerModel>
{
    private readonly ExecutionDbContext _db;
    public CreateManpowerCommandHandler(ExecutionDbContext db) => _db = db;

    public async Task<ManpowerModel> Handle(CreateManpowerCommand request, CancellationToken cancellationToken)
    {
        var r = request.Request;

        var entity = new Himapp.Execution.Domain.Entities.Manpower
        {
            UniqueID = Guid.NewGuid(),
            ProjectID = r.ProjectId,
            SectionID = r.SectionId,
            EntryDate = r.EntryDate,
            Remarks = r.Remarks,
            StateID = 3,
            IsActive = true,
            CreatedBy = r.CreatedBy,
            CreatedDate = DateTimeOffset.UtcNow,
            LastModifiedBy = r.LastModifiedBy,
            LastModifiedDate = DateTimeOffset.UtcNow
        };

        if (r.Details?.Any() == true)
        {
            foreach (var d in r.Details)
            {
                var detail = new ManpowerDetail
                {
                    UniqueID = Guid.NewGuid(),
                    ContractorID = d.ContractorId,
                    ActivityID = d.ActivityId,
                    SkilledCount = d.SkilledCount,
                    UnskilledCount = d.UnskilledCount,
                    OtherCount = d.OtherCount,
                    TotalCount = d.SkilledCount + d.UnskilledCount + d.OtherCount,
                    IsActive = true,
                    CreatedBy = r.CreatedBy,
                    CreatedDate = DateTimeOffset.UtcNow,
                    LastModifiedBy = r.LastModifiedBy,
                    LastModifiedDate = DateTimeOffset.UtcNow,
                    Manpower = entity
                };

                entity.ManpowerDetail?.Add(detail);
            }
        }

        _db.Manpowers.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);

        var details = entity.ManpowerDetail?.Select(d => new ManpowerDetailModel(d.ID, d.UniqueID, d.ContractorID, d.ActivityID, d.SkilledCount, d.UnskilledCount, d.OtherCount, d.TotalCount)).ToArray() ?? Array.Empty<Himapp.Execution.Application.Features.Manpower.Models.ManpowerDetailModel>();

        return new ManpowerModel(entity.ID, entity.UniqueID, entity.ProjectID, entity.SectionID, entity.EntryDate, entity.Remarks, entity.StateID, entity.IsActive, entity.CreatedBy, entity.CreatedDate, entity.LastModifiedBy, entity.LastModifiedDate, details);
    }
}

internal sealed class UpdateManpowerCommandHandler : IRequestHandler<UpdateManpowerCommand, ManpowerModel?>
{
    private readonly ExecutionDbContext _db;
    public UpdateManpowerCommandHandler(ExecutionDbContext db) => _db = db;

    public async Task<ManpowerModel?> Handle(UpdateManpowerCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.Manpowers.Include(x => x.ManpowerDetail).FirstOrDefaultAsync(x => x.ID == request.Id && x.IsActive, cancellationToken);
        if (entity is null) return null;
        var r = request.Request;

        entity.SectionID = r.SectionId;
        entity.EntryDate = r.EntryDate;
        entity.Remarks = r.Remarks;
        entity.StateID = r.StateId;
        entity.IsActive = r.IsActive;
        entity.LastModifiedBy = r.LastModifiedBy;
        entity.LastModifiedDate = DateTimeOffset.UtcNow;

        if (entity.ManpowerDetail != null && entity.ManpowerDetail.Any())
        {
            _db.ManpowerDetails.RemoveRange(entity.ManpowerDetail);
            entity.ManpowerDetail.Clear();
        }

        if (r.Details?.Any() == true)
        {
            foreach (var d in r.Details)
            {
                var detail = new ManpowerDetail
                {
                    UniqueID = Guid.NewGuid(),
                    ContractorID = d.ContractorId,
                    ActivityID = d.ActivityId,
                    SkilledCount = d.SkilledCount,
                    UnskilledCount = d.UnskilledCount,
                    OtherCount = d.OtherCount,
                    TotalCount = d.SkilledCount + d.UnskilledCount + d.OtherCount,
                    IsActive = true,
                    CreatedBy = r.LastModifiedBy,
                    CreatedDate = DateTimeOffset.UtcNow,
                    LastModifiedBy = r.LastModifiedBy,
                    LastModifiedDate = DateTimeOffset.UtcNow,
                    Manpower = entity
                };

                entity.ManpowerDetail?.Add(detail);
            }
        }

        await _db.SaveChangesAsync(cancellationToken);

        var details = entity.ManpowerDetail?.Select(d => new ManpowerDetailModel(d.ID, d.UniqueID, d.ContractorID, d.ActivityID, d.SkilledCount, d.UnskilledCount, d.OtherCount, d.TotalCount)).ToArray() ?? Array.Empty<Himapp.Execution.Application.Features.Manpower.Models.ManpowerDetailModel>();

        return new ManpowerModel(entity.ID, entity.UniqueID, entity.ProjectID, entity.SectionID, entity.EntryDate, entity.Remarks, entity.StateID, entity.IsActive, entity.CreatedBy, entity.CreatedDate, entity.LastModifiedBy, entity.LastModifiedDate, details);
    }
}

internal sealed class DeleteManpowerCommandHandler : IRequestHandler<DeleteManpowerCommand, bool>
{
    private readonly ExecutionDbContext _db;
    public DeleteManpowerCommandHandler(ExecutionDbContext db) => _db = db;

    public async Task<bool> Handle(DeleteManpowerCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.Manpowers.Include(d => d.ManpowerDetail).FirstOrDefaultAsync(x => x.ID == request.Id && x.IsActive, cancellationToken);
        if (entity is null) return false;

        entity.IsActive = false;
        entity.LastModifiedDate = DateTimeOffset.UtcNow;

        if (entity.ManpowerDetail != null)
        {
            foreach (var dd in entity.ManpowerDetail)
            {
                dd.IsActive = false;
                dd.LastModifiedDate = DateTimeOffset.UtcNow;
            }
        }

        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }
}

