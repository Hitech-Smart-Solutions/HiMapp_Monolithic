using Himapp.Execution.Application.Features.DailyProgress.Models;
using Himapp.Execution.Application.Features.DailyProgress.Commands;
using Himapp.Execution.Application.Features.DailyProgress.Queries;
using Himapp.Execution.Domain.Entities;
using Himapp.Execution.Contracts;
using Himapp.SharedKernel.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Himapp.Execution.Application.Features.DailyProgress.Handlers;

internal sealed class DailyProgressHandlers :
    IRequestHandler<GetAllDailyProgressQuery, IReadOnlyCollection<DailyProgressModel>>,
    IRequestHandler<GetDailyProgressByIdQuery, DailyProgressModel?>,
    IRequestHandler<CreateDailyProgressCommand, DailyProgressModel>,
    IRequestHandler<UpdateDailyProgressCommand, DailyProgressModel?>,
    IRequestHandler<DeleteDailyProgressCommand, bool>
{
    private readonly IExecutionDbContext _db;
    private readonly ICurrentUser _currentUser;
    public DailyProgressHandlers(IExecutionDbContext db, ICurrentUser currentUser) => (_db, _currentUser) = (db, currentUser);

    private int CurrentUserId => _currentUser.UserId ?? throw new UnauthorizedAccessException("An authenticated user is required.");

    public async Task<IReadOnlyCollection<DailyProgressModel>> Handle(GetAllDailyProgressQuery request, CancellationToken cancellationToken)
    {
        return await _db.Set<Himapp.Execution.Domain.Entities.DailyProgress>()
            .AsNoTracking()
            .Select(d => new DailyProgressModel(d.ID, d.UniqueID, d.ProjectID, d.ReportDate, d.Hindrances, d.HindranceAudioUrl, d.NextDayPlan, d.Remarks, d.TotalAmount, d.Status, d.IsActive, d.CreatedBy, d.CreatedDate, d.LastModifiedBy, d.LastModifiedDate, Array.Empty<DailyProgressDetailModel>()))
            .ToArrayAsync(cancellationToken);
    }

    public async Task<DailyProgressModel?> Handle(GetDailyProgressByIdQuery request, CancellationToken cancellationToken)
    {
        var d = await _db.Set<Himapp.Execution.Domain.Entities.DailyProgress>()
            .AsNoTracking()
            .Include(x => x.DailyProgressDetail)
            .FirstOrDefaultAsync(x => x.ID == request.Id, cancellationToken);
        if (d is null) return null;

        var details = d.DailyProgressDetail?.Select(dd => new DailyProgressDetailModel(
            dd.ID,
            dd.UniqueID,
            dd.ActivityID,
            dd.Quantity,
            dd.Uom,
            dd.Rate,
            dd.Amount,
            dd.PlanQuantity,
            dd.Variance,
            dd.Remarks)).ToArray() ?? Array.Empty<DailyProgressDetailModel>();

        return new DailyProgressModel(d.ID, d.UniqueID, d.ProjectID, d.ReportDate, d.Hindrances, d.HindranceAudioUrl, d.NextDayPlan, d.Remarks, d.TotalAmount, d.Status, d.IsActive, d.CreatedBy, d.CreatedDate, d.LastModifiedBy, d.LastModifiedDate, details);
    }

    public async Task<DailyProgressModel> Handle(CreateDailyProgressCommand request, CancellationToken cancellationToken)
    {
        var r = request.Request;
        var userId = CurrentUserId;
        var entity = new Himapp.Execution.Domain.Entities.DailyProgress
        {
            UniqueID = Guid.NewGuid(),
            ProjectID = r.ProjectId,
            ReportDate = r.ReportDate,
            Hindrances = r.Hindrances,
            NextDayPlan = r.NextDayPlan,
            Remarks = r.Remarks,
            TotalAmount = 0m,
            Status = "DRAFT",
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
                var detail = new Himapp.Execution.Domain.Entities.DailyProgressDetail
                {
                    UniqueID = Guid.NewGuid(),
                    ActivityID = d.ActivityId,
                    Quantity = d.Quantity,
                    Uom = d.Uom,
                    Rate = d.Rate,
                    PlanQuantity = d.PlanQuantity,
                    Remarks = d.Remarks,
                    IsActive = true,
                    CreatedBy = userId,
                    CreatedDate = DateTimeOffset.UtcNow,
                    LastModifiedBy = userId,
                    LastModifiedDate = DateTimeOffset.UtcNow
                };

                // Amount and Variance are computed in DB
                entity.DailyProgressDetail?.Add(detail);
            }
        }

        _db.Set<Himapp.Execution.Domain.Entities.DailyProgress>().Add(entity);
        await _db.SaveChangesAsync(cancellationToken);

        var details = entity.DailyProgressDetail?.Select(dd => new DailyProgressDetailModel(dd.ID, dd.UniqueID, dd.ActivityID, dd.Quantity, dd.Uom, dd.Rate, dd.Amount, dd.PlanQuantity, dd.Variance, dd.Remarks)).ToArray() ?? Array.Empty<DailyProgressDetailModel>();

        return new DailyProgressModel(entity.ID, entity.UniqueID, entity.ProjectID, entity.ReportDate, entity.Hindrances, entity.HindranceAudioUrl, entity.NextDayPlan, entity.Remarks, entity.TotalAmount, entity.Status, entity.IsActive, entity.CreatedBy, entity.CreatedDate, entity.LastModifiedBy, entity.LastModifiedDate, details);
    }

    public async Task<DailyProgressModel?> Handle(UpdateDailyProgressCommand request, CancellationToken cancellationToken)
    {
        var userId = CurrentUserId;
        var entity = await _db.Set<Himapp.Execution.Domain.Entities.DailyProgress>()
            .Include(d => d.DailyProgressDetail)
            .FirstOrDefaultAsync(x => x.ID == request.Id, cancellationToken);
        if (entity is null) return null;

        entity.Hindrances = request.Request.Hindrances ?? entity.Hindrances;
        entity.NextDayPlan = request.Request.NextDayPlan ?? entity.NextDayPlan;
        entity.Remarks = request.Request.Remarks ?? entity.Remarks;
        entity.Status = request.Request.Status;
        entity.IsActive = request.Request.IsActive;
        entity.LastModifiedBy = userId;
        entity.LastModifiedDate = DateTimeOffset.UtcNow;

        // Remove existing details and add new ones
        if (entity.DailyProgressDetail != null && entity.DailyProgressDetail.Any())
        {
            _db.Set<Himapp.Execution.Domain.Entities.DailyProgressDetail>().RemoveRange(entity.DailyProgressDetail);
            entity.DailyProgressDetail.Clear();
        }

        if (request.Request.Details?.Any() == true)
        {
            foreach (var d in request.Request.Details)
            {
                var detail = new Himapp.Execution.Domain.Entities.DailyProgressDetail
                {
                    UniqueID = Guid.NewGuid(),
                    ActivityID = d.ActivityId,
                    Quantity = d.Quantity,
                    Uom = d.Uom,
                    Rate = d.Rate,
                    PlanQuantity = d.PlanQuantity,
                    Remarks = d.Remarks,
                    IsActive = true,
                    CreatedBy = userId,
                    CreatedDate = DateTimeOffset.UtcNow,
                    LastModifiedBy = userId,
                    LastModifiedDate = DateTimeOffset.UtcNow
                };

                entity.DailyProgressDetail?.Add(detail);
            }
        }

        await _db.SaveChangesAsync(cancellationToken);

        var details = entity.DailyProgressDetail?.Select(dd => new DailyProgressDetailModel(dd.ID, dd.UniqueID, dd.ActivityID, dd.Quantity, dd.Uom, dd.Rate, dd.Amount, dd.PlanQuantity, dd.Variance, dd.Remarks)).ToArray() ?? Array.Empty<DailyProgressDetailModel>();

        return new DailyProgressModel(entity.ID, entity.UniqueID, entity.ProjectID, entity.ReportDate, entity.Hindrances, entity.HindranceAudioUrl, entity.NextDayPlan, entity.Remarks, entity.TotalAmount, entity.Status, entity.IsActive, entity.CreatedBy, entity.CreatedDate, entity.LastModifiedBy, entity.LastModifiedDate, details);
    }

    public async Task<bool> Handle(DeleteDailyProgressCommand request, CancellationToken cancellationToken)
    {
        var userId = CurrentUserId;
        var entity = await _db.Set<Himapp.Execution.Domain.Entities.DailyProgress>()
            .Include(d => d.DailyProgressDetail)
            .FirstOrDefaultAsync(x => x.ID == request.Id, cancellationToken);
        if (entity is null) return false;

        // Soft delete header and child details
        entity.IsActive = false;
        entity.LastModifiedBy = userId;
        entity.LastModifiedDate = DateTimeOffset.UtcNow;

        if (entity.DailyProgressDetail != null)
        {
            foreach (var dd in entity.DailyProgressDetail)
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

