
using Himapp.Execution.Application.Features.DailyLabor.Models;
using Himapp.Execution.Application.Features.Manpower.Commands;
using Himapp.Execution.Application.Features.Manpower.Models;
using Himapp.Execution.Application.Features.Manpower.Queries;
using Himapp.Execution.Contracts;
using Himapp.Execution.Domain.Entities;
using Himapp.SharedKernel.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using NpgsqlTypes;
using System.Data;

namespace Himapp.Execution.Application.Features.Manpower.Handlers;

internal sealed class ManpowerHandlers :
    IRequestHandler<GetAllManpowersQuery, IReadOnlyCollection<ManpowerModel>>,
    IRequestHandler<GetManpowerByIdQuery, ManpowerModel?>,
    IRequestHandler<CreateManpowerCommand, ManpowerModel>,
    IRequestHandler<UpdateManpowerCommand, ManpowerModel?>,
    IRequestHandler<DeleteManpowerCommand, bool>,
    IRequestHandler<DeleteManpowerActionCommand, bool>,
    IRequestHandler<GetManpowerByProjectID, PagedResult<ManpowerModel>>,
    IRequestHandler<GetLastManpowerBySectionIDQuery, ManpowerModel?>
{
    private readonly IExecutionDbContext _db;
    private readonly ICurrentUser _currentUser;
    public ManpowerHandlers(IExecutionDbContext db, ICurrentUser currentUser) => (_db, _currentUser) = (db, currentUser);

    private int CurrentUserId => _currentUser.UserId ?? throw new UnauthorizedAccessException("An authenticated user is required.");

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
                m.ManpowerDetail == null ? Array.Empty<ManpowerDetailModel>() : m.ManpowerDetail.Select(d => new ManpowerDetailModel(
                   d.ID,
                   d.UniqueID,
                   d.ContractorID,
                   d.ActivityID,
                   d.SkilledCount,
                   d.UnskilledCount,
                   d.OtherCount,
                   d.IsDepartment,
                   d.TotalCount
                   )).ToList()
        ))
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
            d.IsDepartment,
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
                    IsDepartment = d.IsDepartment,
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

        var details = entity.ManpowerDetail?.Select(d => new ManpowerDetailModel(d.ID, d.UniqueID, d.ContractorID, d.ActivityID, d.SkilledCount, d.UnskilledCount, d.OtherCount, d.IsDepartment, d.TotalCount)).ToArray() ?? Array.Empty<ManpowerDetailModel>();

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
                    IsDepartment = d.IsDepartment,
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

        var details = entity.ManpowerDetail?.Select(d => new ManpowerDetailModel(d.ID, d.UniqueID, d.ContractorID, d.ActivityID, d.SkilledCount, d.UnskilledCount, d.OtherCount, d.IsDepartment, d.TotalCount)).ToArray() ?? Array.Empty<ManpowerDetailModel>();

        return new ManpowerModel(entity.ID, entity.UniqueID, entity.ProjectID, entity.SectionID, entity.EntryDate, entity.Remarks, entity.StateID, entity.IsActive, entity.CreatedBy, entity.CreatedDate, entity.LastModifiedBy, entity.LastModifiedDate, details);
    }

    public async Task<bool> Handle(DeleteManpowerCommand request, CancellationToken cancellationToken)
    {
        var userId = CurrentUserId;
        var entity = await _db.Set<Himapp.Execution.Domain.Entities.Manpower>().Include(d => d.ManpowerDetail).FirstOrDefaultAsync(x => x.ID == request.Id && x.IsActive, cancellationToken);
        if (entity is null) return false;

        entity.IsActive = false;
        entity.LastModifiedBy = userId;
        entity.LastModifiedDate = DateTimeOffset.UtcNow;

        if (entity.ManpowerDetail != null)
        {
            foreach (var dd in entity.ManpowerDetail)
            {
                dd.IsActive = false;
                dd.LastModifiedBy = userId;
                dd.LastModifiedDate = DateTimeOffset.UtcNow;
            }
        }

        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> Handle(DeleteManpowerActionCommand request, CancellationToken cancellationToken)
    {
        var model = request.addTransactionActionHistoryDTO;
        var entity = await _db.Set<Himapp.Execution.Domain.Entities.Manpower>().Include(d => d.ManpowerDetail).FirstOrDefaultAsync(x => x.ID == model.ProgramRowId, cancellationToken);
        if (entity is null) return false;

        // Mark child details active/inactive
        if (entity.ManpowerDetail != null)
        {
            foreach (var dd in entity.ManpowerDetail)
            {
                dd.IsActive = model.Actions == Actions.Activated ? true : false;
            }
        }

        // Mark main entity active/inactive
        entity.IsActive = model.Actions == Actions.Activated ? true : false;

        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<PagedResult<ManpowerModel>> Handle(GetManpowerByProjectID request, CancellationToken cancellationToken)
    {
        var p = request.SearchParamsProjectWise ?? new SearchParamsProjectWise();

        var query = _db.Set<Himapp.Execution.Domain.Entities.Manpower>().AsNoTracking().Where(x => x.ProjectID == p.ProjectID);
        if (bool.TryParse(p.IsActive, out var isActive)) query = query.Where(x => x.IsActive == isActive);
        var totalCount = await query.CountAsync(cancellationToken);
        var pageSize = p.PageSize > 0 ? p.PageSize : totalCount;
        var items = await query.OrderBy(x => x.ID).Skip(Math.Max(p.PageIndex, 0) * pageSize).Take(pageSize)
            .Select(x => new ManpowerModel(x.ID, x.UniqueID, x.ProjectID, x.SectionID, x.EntryDate, x.Remarks, x.StateID, x.IsActive, x.CreatedBy, x.CreatedDate, x.LastModifiedBy, x.LastModifiedDate, Array.Empty<ManpowerDetailModel>()))
            .ToArrayAsync(cancellationToken);
        return new PagedResult<ManpowerModel>(items, totalCount);
    }

    public async Task<ManpowerModel?> Handle(GetLastManpowerBySectionIDQuery request, CancellationToken cancellationToken)
    {
        var manpower = await _db.Set<Domain.Entities.Manpower>().AsNoTracking().Include(x => x.ManpowerDetail).Where(x => x.ProjectID == request.ProjectId && x.SectionID == request.SectionId && x.IsActive)
            .OrderByDescending(x => x.EntryDate)
            .ThenByDescending(x => x.ID)
            .FirstOrDefaultAsync(cancellationToken);

        if (manpower == null)
            return null;

    var details = manpower.ManpowerDetail?
        .Where(x => x.IsActive)
        .Select(d => new ManpowerDetailModel(
            d.ID,
            d.UniqueID,
            d.ContractorID,
            d.ActivityID,
            d.SkilledCount,
            d.UnskilledCount,
            d.OtherCount,
            d.IsDepartment,
            d.TotalCount))
        .ToList()
        ?? new List<ManpowerDetailModel>();

    return new ManpowerModel(
        manpower.ID,
        manpower.UniqueID,
        manpower.ProjectID,
        manpower.SectionID,
        manpower.EntryDate,
        manpower.Remarks,
        manpower.StateID,
        manpower.IsActive,
        manpower.CreatedBy,
        manpower.CreatedDate,
        manpower.LastModifiedBy,
        manpower.LastModifiedDate,
        details);
    }
}

