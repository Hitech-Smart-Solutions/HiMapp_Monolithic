
using Himapp.Execution.Application.Features.Manpower.Models;
using Himapp.Execution.Application.Features.Manpower.Commands;
using Himapp.Execution.Application.Features.Manpower.Queries;
using Himapp.Execution.Domain.Entities;
using Himapp.Execution.Contracts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Himapp.Execution.Application.Features.Manpower.Handlers;

internal sealed class ManpowerHandlers :
    IRequestHandler<GetAllManpowersQuery, IReadOnlyCollection<ManpowerModel>>,
    IRequestHandler<GetManpowerByIdQuery, ManpowerModel?>,
    IRequestHandler<CreateManpowerCommand, ManpowerModel>,
    IRequestHandler<UpdateManpowerCommand, ManpowerModel?>,
    IRequestHandler<DeleteManpowerCommand, bool>
{
    private readonly IExecutionDbContext _db;
    public ManpowerHandlers(IExecutionDbContext db) => _db = db;

    public async Task<IReadOnlyCollection<ManpowerModel>> Handle(GetAllManpowersQuery request, CancellationToken cancellationToken)
    {
        return await _db.Set<Himapp.Execution.Domain.Entities.Manpower>()
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

    public async Task<ManpowerModel?> Handle(GetManpowerByIdQuery request, CancellationToken cancellationToken)
    {
        var m = await _db.Set<Himapp.Execution.Domain.Entities.Manpower>()
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
                var detail = new Himapp.Execution.Domain.Entities.ManpowerDetail
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

        _db.Set<Himapp.Execution.Domain.Entities.Manpower>().Add(entity);
        await _db.SaveChangesAsync(cancellationToken);

        var details = entity.ManpowerDetail?.Select(d => new ManpowerDetailModel(d.ID, d.UniqueID, d.ContractorID, d.ActivityID, d.SkilledCount, d.UnskilledCount, d.OtherCount, d.TotalCount)).ToArray() ?? Array.Empty<ManpowerDetailModel>();

        return new ManpowerModel(entity.ID, entity.UniqueID, entity.ProjectID, entity.SectionID, entity.EntryDate, entity.Remarks, entity.StateID, entity.IsActive, entity.CreatedBy, entity.CreatedDate, entity.LastModifiedBy, entity.LastModifiedDate, details);
    }

    public async Task<ManpowerModel?> Handle(UpdateManpowerCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.Set<Himapp.Execution.Domain.Entities.Manpower>().Include(x => x.ManpowerDetail).FirstOrDefaultAsync(x => x.ID == request.Id && x.IsActive, cancellationToken);
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
            _db.Set<Himapp.Execution.Domain.Entities.ManpowerDetail>().RemoveRange(entity.ManpowerDetail);
            entity.ManpowerDetail.Clear();
        }

        if (r.Details?.Any() == true)
        {
            foreach (var d in r.Details)
            {
                var detail = new Himapp.Execution.Domain.Entities.ManpowerDetail
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

        var details = entity.ManpowerDetail?.Select(d => new ManpowerDetailModel(d.ID, d.UniqueID, d.ContractorID, d.ActivityID, d.SkilledCount, d.UnskilledCount, d.OtherCount, d.TotalCount)).ToArray() ?? Array.Empty<ManpowerDetailModel>();

        return new ManpowerModel(entity.ID, entity.UniqueID, entity.ProjectID, entity.SectionID, entity.EntryDate, entity.Remarks, entity.StateID, entity.IsActive, entity.CreatedBy, entity.CreatedDate, entity.LastModifiedBy, entity.LastModifiedDate, details);
    }

    public async Task<bool> Handle(DeleteManpowerCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.Set<Himapp.Execution.Domain.Entities.Manpower>().Include(d => d.ManpowerDetail).FirstOrDefaultAsync(x => x.ID == request.Id && x.IsActive, cancellationToken);
        if (entity is null) return false;

        entity.IsActive = false;
        entity.LastModifiedBy = 0;
        entity.LastModifiedDate = DateTimeOffset.UtcNow;

        if (entity.ManpowerDetail != null)
        {
            foreach (var dd in entity.ManpowerDetail)
            {
                dd.IsActive = false;
                dd.LastModifiedBy = 0;
                dd.LastModifiedDate = DateTimeOffset.UtcNow;
            }
        }

        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }
}

