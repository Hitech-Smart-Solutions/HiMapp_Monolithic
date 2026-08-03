using Himapp.Execution.Application.Features.DailyDepartmentalLabourSlip.Models;
using Himapp.Execution.Application.Features.DailyDepartmentalLabourSlip.Commands;
using Himapp.Execution.Application.Features.DailyDepartmentalLabourSlip.Queries;
using Himapp.Execution.Domain.Entities;
using Himapp.Execution.Contracts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Himapp.Execution.Application.Features.DailyDepartmentalLabourSlip.Handlers;

internal sealed class DailyDepartmentalLabourSlipHandlers :
    IRequestHandler<GetAllDailyDepartmentalLabourSlipsQuery, IEnumerable<DailyDepartmentalLabourSlipModel>>,
    IRequestHandler<GetDailyDepartmentalLabourSlipByIdQuery, DailyDepartmentalLabourSlipModel?>,
    IRequestHandler<CreateDailyDepartmentalLabourSlipCommand, DailyDepartmentalLabourSlipModel>,
    IRequestHandler<UpdateDailyDepartmentalLabourSlipCommand, DailyDepartmentalLabourSlipModel?>,
    IRequestHandler<DeleteDailyDepartmentalLabourSlipCommand, bool>
{
    private readonly IExecutionDbContext _db;
    public DailyDepartmentalLabourSlipHandlers(IExecutionDbContext db) => _db = db;

    public async Task<IEnumerable<DailyDepartmentalLabourSlipModel>> Handle(GetAllDailyDepartmentalLabourSlipsQuery request, CancellationToken cancellationToken)
    {
        return await _db.Set<Himapp.Execution.Domain.Entities.DailyDepartmentalLabourSlip>()
            .AsNoTracking()
            .Where(d => d.IsActive)
            .Select(d => new DailyDepartmentalLabourSlipModel(
                d.ID,
                d.UniqueID,
                d.ProjectID,
                d.SlipDate,
                d.Remarks,
                d.IsActive,
                d.CreatedBy,
                d.CreatedDate,
                d.LastModifiedBy,
                d.LastModifiedDate,
                Array.Empty<DailyDepartmentalLabourSlipDetailsModel>()))
            .ToArrayAsync(cancellationToken);
    }

    public async Task<DailyDepartmentalLabourSlipModel?> Handle(GetDailyDepartmentalLabourSlipByIdQuery request, CancellationToken cancellationToken)
    {
        var d = await _db.Set<Himapp.Execution.Domain.Entities.DailyDepartmentalLabourSlip>()
            .AsNoTracking()
            .Include(x => x.DailyDepartmentalLabourSlipDetails)
            .FirstOrDefaultAsync(x => x.ID == request.Id && x.IsActive, cancellationToken);
        if (d is null) return null;

        var details = d.DailyDepartmentalLabourSlipDetails?.Select(dd => new DailyDepartmentalLabourSlipDetailsModel(
            dd.ID,
            dd.UniqueID,
            dd.LabourCategoryTypeID,
            dd.NumOfLabour,
            dd.FromTime,
            dd.TOTime,
            dd.LunchHour,
            dd.WorkingHours,
            dd.WorkLocationID,
            dd.ActivityCategoryID,
            dd.ActivityDetails,
            dd.UOMID,
            dd.Quantity,
            dd.DebitPartyID,
            dd.Remarks)).ToArray() ?? Array.Empty<DailyDepartmentalLabourSlipDetailsModel>();

        return new DailyDepartmentalLabourSlipModel(d.ID, d.UniqueID, d.ProjectID, d.SlipDate, d.Remarks, d.IsActive, d.CreatedBy, d.CreatedDate, d.LastModifiedBy, d.LastModifiedDate, details);
    }

    public async Task<DailyDepartmentalLabourSlipModel> Handle(CreateDailyDepartmentalLabourSlipCommand request, CancellationToken cancellationToken)
    {
        var r = request.Request;

        var entity = new Himapp.Execution.Domain.Entities.DailyDepartmentalLabourSlip
        {
            UniqueID = Guid.NewGuid(),
            ProjectID = r.ProjectId,
            SlipDate = r.SlipDate?.UtcDateTime,
            Remarks = r.Remarks,
            IsActive = true,
            CreatedBy = r.CreatedBy,
            CreatedDate = DateTime.UtcNow,
            LastModifiedBy = r.LastModifiedBy,
            LastModifiedDate = DateTime.UtcNow
        };

        if (r.Details?.Any() == true)
        {
            foreach (var d in r.Details)
            {
                var detail = new Himapp.Execution.Domain.Entities.DailyDepartmentalLabourSlipDetails
                {
                    UniqueID = Guid.NewGuid(),
                    LabourCategoryTypeID = d.LabourCategoryTypeId,
                    NumOfLabour = d.NumOfLabour,
                    FromTime = d.FromTime,
                    TOTime = d.ToTime,
                    LunchHour = d.LunchHour,
                    WorkLocationID = d.WorkLocationId,
                    ActivityCategoryID = d.ActivityCategoryId,
                    ActivityDetails = d.ActivityDetails,
                    UOMID = d.UomId,
                    Quantity = d.Quantity,
                    DebitPartyID = d.DebitPartyId,
                    Remarks = d.Remarks,
                    IsActive = true,
                    CreatedBy = r.CreatedBy,
                    CreatedDate = DateTime.UtcNow,
                    LastModifiedBy = r.LastModifiedBy,
                    LastModifiedDate = DateTime.UtcNow
                };

                entity.DailyDepartmentalLabourSlipDetails?.Add(detail);
            }
        }

        _db.Set<Himapp.Execution.Domain.Entities.DailyDepartmentalLabourSlip>().Add(entity);
        await _db.SaveChangesAsync(cancellationToken);

        var details = entity.DailyDepartmentalLabourSlipDetails?.Select(dd => new DailyDepartmentalLabourSlipDetailsModel(
            dd.ID,
            dd.UniqueID,
            dd.LabourCategoryTypeID,
            dd.NumOfLabour,
            dd.FromTime,
            dd.TOTime,
            dd.LunchHour,
            dd.WorkingHours,
            dd.WorkLocationID,
            dd.ActivityCategoryID,
            dd.ActivityDetails,
            dd.UOMID,
            dd.Quantity,
            dd.DebitPartyID,
            dd.Remarks)).ToArray() ?? Array.Empty<DailyDepartmentalLabourSlipDetailsModel>();

        return new DailyDepartmentalLabourSlipModel(entity.ID, entity.UniqueID, entity.ProjectID, entity.SlipDate, entity.Remarks, entity.IsActive, entity.CreatedBy, entity.CreatedDate, entity.LastModifiedBy, entity.LastModifiedDate, details);
    }

    public async Task<DailyDepartmentalLabourSlipModel?> Handle(UpdateDailyDepartmentalLabourSlipCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.Set<Himapp.Execution.Domain.Entities.DailyDepartmentalLabourSlip>()
            .Include(d => d.DailyDepartmentalLabourSlipDetails)
            .FirstOrDefaultAsync(x => x.ID == request.Id && x.IsActive, cancellationToken);
        if (entity is null) return null;

        var r = request.Request;

        entity.ProjectID = r.ProjectId;
        entity.SlipDate = r.SlipDate?.UtcDateTime ?? entity.SlipDate;
        entity.Remarks = r.Remarks ?? entity.Remarks;
        entity.LastModifiedBy = r.LastModifiedBy;
        entity.LastModifiedDate = DateTime.UtcNow;

        // Remove existing details and add new ones
        if (entity.DailyDepartmentalLabourSlipDetails != null && entity.DailyDepartmentalLabourSlipDetails.Any())
        {
            _db.Set<Himapp.Execution.Domain.Entities.DailyDepartmentalLabourSlipDetails>().RemoveRange(entity.DailyDepartmentalLabourSlipDetails);
            entity.DailyDepartmentalLabourSlipDetails.Clear();
        }

        if (r.Details?.Any() == true)
        {
            foreach (var d in r.Details)
            {
                var detail = new Himapp.Execution.Domain.Entities.DailyDepartmentalLabourSlipDetails
                {
                    UniqueID = Guid.NewGuid(),
                    LabourCategoryTypeID = d.LabourCategoryTypeId,
                    NumOfLabour = d.NumOfLabour,
                    FromTime = d.FromTime,
                    TOTime = d.ToTime,
                    LunchHour = d.LunchHour,
                    WorkLocationID = d.WorkLocationId,
                    ActivityCategoryID = d.ActivityCategoryId,
                    ActivityDetails = d.ActivityDetails,
                    UOMID = d.UomId,
                    Quantity = d.Quantity,
                    DebitPartyID = d.DebitPartyId,
                    Remarks = d.Remarks,
                    IsActive = true,
                    CreatedBy = r.LastModifiedBy,
                    CreatedDate = DateTime.UtcNow,
                    LastModifiedBy = r.LastModifiedBy,
                    LastModifiedDate = DateTime.UtcNow
                };

                entity.DailyDepartmentalLabourSlipDetails?.Add(detail);
            }
        }

        await _db.SaveChangesAsync(cancellationToken);

        var details = entity.DailyDepartmentalLabourSlipDetails?.Select(dd => new DailyDepartmentalLabourSlipDetailsModel(
            dd.ID,
            dd.UniqueID,
            dd.LabourCategoryTypeID,
            dd.NumOfLabour,
            dd.FromTime,
            dd.TOTime,
            dd.LunchHour,
            dd.WorkingHours,
            dd.WorkLocationID,
            dd.ActivityCategoryID,
            dd.ActivityDetails,
            dd.UOMID,
            dd.Quantity,
            dd.DebitPartyID,
            dd.Remarks)).ToArray() ?? Array.Empty<DailyDepartmentalLabourSlipDetailsModel>();

        return new DailyDepartmentalLabourSlipModel(entity.ID, entity.UniqueID, entity.ProjectID, entity.SlipDate, entity.Remarks, entity.IsActive, entity.CreatedBy, entity.CreatedDate, entity.LastModifiedBy, entity.LastModifiedDate, details);
    }

    public async Task<bool> Handle(DeleteDailyDepartmentalLabourSlipCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.Set<Himapp.Execution.Domain.Entities.DailyDepartmentalLabourSlip>()
            .Include(d => d.DailyDepartmentalLabourSlipDetails)
            .FirstOrDefaultAsync(x => x.ID == request.Id && x.IsActive, cancellationToken);
        if (entity is null) return false;

        // Soft delete header and child details
        entity.IsActive = false;
        entity.LastModifiedDate = DateTime.UtcNow;

        if (entity.DailyDepartmentalLabourSlipDetails != null)
        {
            foreach (var detail in entity.DailyDepartmentalLabourSlipDetails)
            {
                detail.IsActive = false;
                detail.LastModifiedDate = DateTime.UtcNow;
            }
        }

        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }
}
