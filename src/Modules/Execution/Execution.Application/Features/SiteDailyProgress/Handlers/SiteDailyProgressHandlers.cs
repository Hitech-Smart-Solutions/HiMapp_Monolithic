using Himapp.Execution.Application.Features.SiteDailyProgress.Models;
using Himapp.Execution.Application.Features.SiteDailyProgress.Commands;
using Himapp.Execution.Application.Features.SiteDailyProgress.Queries;
using Himapp.Execution.Domain.Entities;
using Himapp.Execution.Contracts;
using Himapp.SharedKernel.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Himapp.Execution.Application.Features.SiteDailyProgress.Handlers;

internal sealed class SiteDailyProgressHandlers :
    IRequestHandler<GetAllSiteDailyProgressesQuery, IEnumerable<SiteDailyProgressModel>>,
    IRequestHandler<GetSiteDailyProgressByIdQuery, SiteDailyProgressModel?>,
    IRequestHandler<CreateSiteDailyProgressCommand, SiteDailyProgressModel>,
    IRequestHandler<UpdateSiteDailyProgressCommand, SiteDailyProgressModel?>,
    IRequestHandler<DeleteSiteDailyProgressCommand, bool>
{
    private readonly IExecutionDbContext _db;
    private readonly ICurrentUser _currentUser;
    public SiteDailyProgressHandlers(IExecutionDbContext db, ICurrentUser currentUser) => (_db, _currentUser) = (db, currentUser);

    private int CurrentUserId => _currentUser.UserId ?? throw new UnauthorizedAccessException("An authenticated user is required.");

    public async Task<IEnumerable<SiteDailyProgressModel>> Handle(GetAllSiteDailyProgressesQuery request, CancellationToken cancellationToken)
    {
        return await _db.Set<Himapp.Execution.Domain.Entities.SiteDailyProgress>()
            .AsNoTracking()
            .Where(d => d.IsActive)
            .Select(d => new SiteDailyProgressModel(d.ID, d.ProjectID, d.ReportDate, d.Remarks, d.IsActive, d.CreatedBy, d.CreatedDate, d.LastModifiedBy, d.LastModifiedDate, Array.Empty<SiteDailyProgressDetailModel>()))
            .ToArrayAsync(cancellationToken);
    }

    public async Task<SiteDailyProgressModel?> Handle(GetSiteDailyProgressByIdQuery request, CancellationToken cancellationToken)
    {
        var d = await _db.Set<Himapp.Execution.Domain.Entities.SiteDailyProgress>()
            .AsNoTracking()
            .Include(x => x.SiteDailyProgressDetail)
            .FirstOrDefaultAsync(x => x.ID == request.Id && x.IsActive, cancellationToken);
        if (d is null) return null;

        var details = d.SiteDailyProgressDetail?.Select(dd => new SiteDailyProgressDetailModel(
            dd.ID,
            dd.UniqueId,
            dd.ActivityID,
            dd.Quantity,
            dd.UOMID,
            dd.Rate,
            dd.Amount,
            dd.PlanQuantity,
            dd.Variance,
            dd.Remarks)).ToArray() ?? Array.Empty<SiteDailyProgressDetailModel>();

        return new SiteDailyProgressModel(d.ID, d.ProjectID, d.ReportDate, d.Remarks, d.IsActive, d.CreatedBy, d.CreatedDate, d.LastModifiedBy, d.LastModifiedDate, details);
    }

    public async Task<SiteDailyProgressModel> Handle(CreateSiteDailyProgressCommand request, CancellationToken cancellationToken)
    {
        var r = request.Request;
        var userId = CurrentUserId;

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
            CreatedBy = userId,
            CreatedDate = DateTimeOffset.UtcNow,
            LastModifiedBy = userId,
            LastModifiedDate = DateTimeOffset.UtcNow
        };

        if (r.Details?.Any() == true)
        {
            foreach (var d in r.Details)
            {
                var detail = new Himapp.Execution.Domain.Entities.SiteDailyProgressDetail
                {
                    UniqueId = Guid.NewGuid(),
                    ActivityID = d.ActivityId,
                    Quantity = d.Quantity,
                    UOMID = d.UomId,
                    Rate = d.Rate,
                    PlanQuantity = d.PlanQuantity,
                    Remarks = d.Remarks,
                    IsActive = true,
                    CreatedBy = userId,
                    CreatedDate = DateTimeOffset.UtcNow,
                    LastModifiedBy = userId,
                    LastModifiedDate = DateTimeOffset.UtcNow
                };

                entity.SiteDailyProgressDetail?.Add(detail);
            }
        }

        _db.Set<Himapp.Execution.Domain.Entities.SiteDailyProgress>().Add(entity);
        await _db.SaveChangesAsync(cancellationToken);

        var details = entity.SiteDailyProgressDetail?.Select(dd => new SiteDailyProgressDetailModel(dd.ID, dd.UniqueId, dd.ActivityID, dd.Quantity, dd.UOMID, dd.Rate, dd.Amount, dd.PlanQuantity, dd.Variance, dd.Remarks)).ToArray() ?? Array.Empty<SiteDailyProgressDetailModel>();

        return new SiteDailyProgressModel(entity.ID, entity.ProjectID, entity.ReportDate, entity.Remarks, entity.IsActive, entity.CreatedBy, entity.CreatedDate, entity.LastModifiedBy, entity.LastModifiedDate, details);
    }

    public async Task<SiteDailyProgressModel?> Handle(UpdateSiteDailyProgressCommand request, CancellationToken cancellationToken)
    {
        var userId = CurrentUserId;
        var entity = await _db.Set<Himapp.Execution.Domain.Entities.SiteDailyProgress>()
            .Include(d => d.SiteDailyProgressDetail)
            .FirstOrDefaultAsync(x => x.ID == request.Id && x.IsActive, cancellationToken);
        if (entity is null) return null;

        var r = request.Request;

        entity.ProjectID = r.ProjectId;
        entity.ReportDate = r.ReportDate.HasValue ? DateOnly.FromDateTime(r.ReportDate.Value.UtcDateTime) : entity.ReportDate;
        entity.Remarks = r.Remarks ?? entity.Remarks;
        entity.LastModifiedBy = userId;
        entity.LastModifiedDate = DateTimeOffset.UtcNow;

        // Remove existing details and add new ones
        if (entity.SiteDailyProgressDetail != null && entity.SiteDailyProgressDetail.Any())
        {
            _db.Set<Himapp.Execution.Domain.Entities.SiteDailyProgressDetail>().RemoveRange(entity.SiteDailyProgressDetail);
            entity.SiteDailyProgressDetail.Clear();
        }

        if (r.Details?.Any() == true)
        {
            foreach (var d in r.Details)
            {
                var detail = new Himapp.Execution.Domain.Entities.SiteDailyProgressDetail
                {
                    UniqueId = Guid.NewGuid(),
                    ActivityID = d.ActivityId,
                    Quantity = d.Quantity,
                    UOMID = d.UomId,
                    Rate = d.Rate,
                    PlanQuantity = d.PlanQuantity,
                    Remarks = d.Remarks,
                    IsActive = true,
                    CreatedBy = userId,
                    CreatedDate = DateTimeOffset.UtcNow,
                    LastModifiedBy = userId,
                    LastModifiedDate = DateTimeOffset.UtcNow
                };

                entity.SiteDailyProgressDetail?.Add(detail);
            }
        }

        await _db.SaveChangesAsync(cancellationToken);

        var details = entity.SiteDailyProgressDetail?.Select(dd => new SiteDailyProgressDetailModel(dd.ID, dd.UniqueId, dd.ActivityID, dd.Quantity, dd.UOMID, dd.Rate, dd.Amount, dd.PlanQuantity, dd.Variance, dd.Remarks)).ToArray() ?? Array.Empty<SiteDailyProgressDetailModel>();

        return new SiteDailyProgressModel(entity.ID, entity.ProjectID, entity.ReportDate, entity.Remarks, entity.IsActive, entity.CreatedBy, entity.CreatedDate, entity.LastModifiedBy, entity.LastModifiedDate, details);
    }

    public async Task<bool> Handle(DeleteSiteDailyProgressCommand request, CancellationToken cancellationToken)
    {
        var userId = CurrentUserId;
        var entity = await _db.Set<Himapp.Execution.Domain.Entities.SiteDailyProgress>()
            .Include(d => d.SiteDailyProgressDetail)
            .FirstOrDefaultAsync(x => x.ID == request.Id && x.IsActive, cancellationToken);
        if (entity is null) return false;

        // Soft delete header and child details
        entity.IsActive = false;
        entity.LastModifiedBy = userId;
        entity.LastModifiedDate = DateTimeOffset.UtcNow;

        if (entity.SiteDailyProgressDetail != null)
        {
            foreach (var dd in entity.SiteDailyProgressDetail)
            {
                dd.IsActive = false;
                dd.LastModifiedBy = userId;
                dd.LastModifiedDate = DateTimeOffset.UtcNow;
            }
        }

        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }
}
