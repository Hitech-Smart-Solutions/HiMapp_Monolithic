using Himapp.Execution.Application.Features.Planning.Models;
using Himapp.Execution.Application.Features.Planning.Commands;
using Himapp.Execution.Application.Features.Planning.Queries;
using Himapp.Execution.Domain.Entities;
using Himapp.Execution.Contracts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Himapp.Execution.Application.Features.Planning.Handlers;

internal sealed class PlanningHandlers :
    IRequestHandler<GetAllPlanningsQuery, IReadOnlyCollection<PlanningModel>>,
    IRequestHandler<GetPlanningByIdQuery, PlanningModel?>,
    IRequestHandler<CreatePlanningCommand, PlanningModel>,
    IRequestHandler<UpdatePlanningCommand, PlanningModel?>,
    IRequestHandler<DeletePlanningCommand, bool>
{
    private readonly IExecutionDbContext _db;
    public PlanningHandlers(IExecutionDbContext db) => _db = db;

    public async Task<IReadOnlyCollection<PlanningModel>> Handle(GetAllPlanningsQuery request, CancellationToken cancellationToken)
    {
        return await _db.Set<Himapp.Execution.Domain.Entities.Planning>()
            .AsNoTracking()
            .Select(p => new PlanningModel(p.ID, p.UniqueID, p.ProjectID, p.PlanType, p.StartDate, p.EndDate, p.Remarks, p.Status, p.IsActive, p.CreatedBy, p.CreatedDate, p.LastModifiedBy, p.LastModifiedDate, Array.Empty<PlanningDetailModel>()))
            .ToArrayAsync(cancellationToken);
    }

    public async Task<PlanningModel?> Handle(GetPlanningByIdQuery request, CancellationToken cancellationToken)
    {
        var p = await _db.Set<Himapp.Execution.Domain.Entities.Planning>()
            .AsNoTracking()
            .Include(x => x.PlanningDetail)
            .FirstOrDefaultAsync(x => x.ID == request.Id, cancellationToken);
        if (p is null) return null;

        var details = p.PlanningDetail?.Select(pd => new PlanningDetailModel(
            pd.ID,
            pd.UniqueID,
            pd.AreaID,
            pd.ActivityID,
            pd.TargetQuantity,
            pd.UOMID,
            pd.Remarks)).ToArray() ?? Array.Empty<PlanningDetailModel>();

        return new PlanningModel(p.ID, p.UniqueID, p.ProjectID, p.PlanType, p.StartDate, p.EndDate, p.Remarks, p.Status, p.IsActive, p.CreatedBy, p.CreatedDate, p.LastModifiedBy, p.LastModifiedDate, details);
    }

    public async Task<PlanningModel> Handle(CreatePlanningCommand request, CancellationToken cancellationToken)
    {
        var r = request.Request;
        var entity = new Himapp.Execution.Domain.Entities.Planning
        {
            UniqueID = Guid.NewGuid(),
            ProjectID = r.ProjectId,
            PlanType = r.PlanType,
            StartDate = r.StartDate,
            EndDate = r.EndDate,
            Remarks = r.Remarks,
            Status = "DRAFT",
            IsActive = true,
            CreatedBy = 0,
            CreatedDate = DateTimeOffset.UtcNow,
            LastModifiedBy = 0,
            LastModifiedDate = DateTimeOffset.UtcNow
        };

        if (r.Details?.Any() == true)
        {
            foreach (var d in r.Details)
            {
                var detail = new Himapp.Execution.Domain.Entities.PlanningDetail
                {
                    UniqueID = Guid.NewGuid(),
                    AreaID = d.AreaId,
                    ActivityID = d.ActivityId,
                    TargetQuantity = d.TargetQuantity,
                    UOMID = d.UomId,
                    Remarks = d.Remarks,
                    IsActive = true,
                    CreatedBy = 0,
                    CreatedDate = DateTimeOffset.UtcNow,
                    LastModifiedBy = 0,
                    LastModifiedDate = DateTimeOffset.UtcNow
                };

                entity.PlanningDetail?.Add(detail);
            }
        }

        _db.Set<Himapp.Execution.Domain.Entities.Planning>().Add(entity);
        await _db.SaveChangesAsync(cancellationToken);

        var details = entity.PlanningDetail?.Select(pd => new PlanningDetailModel(pd.ID, pd.UniqueID, pd.AreaID, pd.ActivityID, pd.TargetQuantity, pd.UOMID, pd.Remarks)).ToArray() ?? Array.Empty<PlanningDetailModel>();

        return new PlanningModel(entity.ID, entity.UniqueID, entity.ProjectID, entity.PlanType, entity.StartDate, entity.EndDate, entity.Remarks, entity.Status, entity.IsActive, entity.CreatedBy, entity.CreatedDate, entity.LastModifiedBy, entity.LastModifiedDate, details);
    }

    public async Task<PlanningModel?> Handle(UpdatePlanningCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.Set<Himapp.Execution.Domain.Entities.Planning>()
            .Include(d => d.PlanningDetail)
            .FirstOrDefaultAsync(x => x.ID == request.Id, cancellationToken);
        if (entity is null) return null;

        entity.Remarks = request.Request.Remarks ?? entity.Remarks;
        entity.Status = request.Request.Status;
        entity.IsActive = request.Request.IsActive;
        entity.LastModifiedBy = 0;
        entity.LastModifiedDate = DateTimeOffset.UtcNow;

        // Remove existing details and add new ones
        if (entity.PlanningDetail != null && entity.PlanningDetail.Any())
        {
            _db.Set<Himapp.Execution.Domain.Entities.PlanningDetail>().RemoveRange(entity.PlanningDetail);
            entity.PlanningDetail.Clear();
        }

        if (request.Request.Details?.Any() == true)
        {
            foreach (var d in request.Request.Details)
            {
                var detail = new Himapp.Execution.Domain.Entities.PlanningDetail
                {
                    UniqueID = Guid.NewGuid(),
                    AreaID = d.AreaId,
                    ActivityID = d.ActivityId,
                    TargetQuantity = d.TargetQuantity,
                    UOMID = d.UomId,
                    Remarks = d.Remarks,
                    IsActive = true,
                    CreatedBy = 0,
                    CreatedDate = DateTimeOffset.UtcNow,
                    LastModifiedBy = 0,
                    LastModifiedDate = DateTimeOffset.UtcNow
                };

                entity.PlanningDetail?.Add(detail);
            }
        }

        await _db.SaveChangesAsync(cancellationToken);

        var details = entity.PlanningDetail?.Select(pd => new PlanningDetailModel(pd.ID, pd.UniqueID, pd.AreaID, pd.ActivityID, pd.TargetQuantity, pd.UOMID, pd.Remarks)).ToArray() ?? Array.Empty<PlanningDetailModel>();

        return new PlanningModel(entity.ID, entity.UniqueID, entity.ProjectID, entity.PlanType, entity.StartDate, entity.EndDate, entity.Remarks, entity.Status, entity.IsActive, entity.CreatedBy, entity.CreatedDate, entity.LastModifiedBy, entity.LastModifiedDate, details);
    }

    public async Task<bool> Handle(DeletePlanningCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.Set<Himapp.Execution.Domain.Entities.Planning>()
            .Include(d => d.PlanningDetail)
            .FirstOrDefaultAsync(x => x.ID == request.Id, cancellationToken);
        if (entity is null) return false;

        // Soft delete header and child details
        entity.IsActive = false;
        entity.LastModifiedBy = 0;
        entity.LastModifiedDate = DateTimeOffset.UtcNow;

        if (entity.PlanningDetail != null)
        {
            foreach (var pd in entity.PlanningDetail)
            {
                pd.IsActive = false;
                pd.LastModifiedBy = 0;
                pd.LastModifiedDate = DateTimeOffset.UtcNow;
            }
        }

        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }
}

