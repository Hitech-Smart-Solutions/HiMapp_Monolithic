using Himapp.Execution.Application.Features.DailyProgress.Commands;
using Himapp.Execution.Application.Features.DailyProgress.Models;
using Himapp.Execution.Application.Features.DailyProgress.Queries;
using Himapp.Execution.Application.Features.Planning.Queries;
using Himapp.Execution.Contracts;
using Himapp.Execution.Domain.Entities;
using Himapp.SharedKernel.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Data;

namespace Himapp.Execution.Application.Features.DailyProgress.Handlers;

internal sealed class DailyProgressHandlers :
    IRequestHandler<GetAllDailyProgressQuery, IReadOnlyCollection<DailyProgressModel>>,
    IRequestHandler<GetDailyProgressByIdQuery, DailyProgressModel?>,
    IRequestHandler<CreateDailyProgressCommand, DailyProgressModel>,
    IRequestHandler<UpdateDailyProgressCommand, DailyProgressModel?>,
    IRequestHandler<DeleteDailyProgressCommand, bool>,
    IRequestHandler<GetDailyProgressListByProjectQuery, DataSet>
{
    private readonly IExecutionDbContext _db;
    private readonly ICurrentUser _currentUser;
    public DailyProgressHandlers(IExecutionDbContext db, ICurrentUser currentUser) => (_db, _currentUser) = (db, currentUser);

    private int CurrentUserId => _currentUser.UserId ?? throw new UnauthorizedAccessException("An authenticated user is required.");

    public async Task<IReadOnlyCollection<DailyProgressModel>> Handle(GetAllDailyProgressQuery request, CancellationToken cancellationToken)
    {
        return await _db.Set<Himapp.Execution.Domain.Entities.DailyProgress>()
            .AsNoTracking()
            .Select(d => new DailyProgressModel(d.ID, d.UniqueID, d.ProjectID, d.ReportDate, d.NextDayPlan, d.Remarks, d.TotalAmount, d.StatusID, d.IsActive, d.CreatedBy, d.CreatedDate, d.LastModifiedBy, d.LastModifiedDate, Array.Empty<DailyProgressDetailModel>(), Array.Empty<DailyProgressHindranceModel>(), Array.Empty<DailyProgressPhotoModel>()))
            .ToArrayAsync(cancellationToken);
    }

    public async Task<DailyProgressModel?> Handle(GetDailyProgressByIdQuery request, CancellationToken cancellationToken)
    {
        var d = await _db.Set<Himapp.Execution.Domain.Entities.DailyProgress>()
            .AsNoTracking()
            .Include(x => x.DailyProgressDetail)
            .Include(x => x.DailyProgressHindrance)
            .Include(x => x.DailyProgressPhoto)
            .FirstOrDefaultAsync(x => x.ID == request.Id, cancellationToken);
        if (d is null) return null;

        var details = d.DailyProgressDetail?.Select(dd => new DailyProgressDetailModel(
            dd.ID,
            dd.UniqueID,
            dd.ActivityID,
            dd.Quantity,
            dd.UOMID,
            dd.Rate,
            dd.Amount,
            dd.PlanQuantity,
            dd.Variance,
            dd.Remarks)).ToArray() ?? Array.Empty<DailyProgressDetailModel>();

        var hindrances = d.DailyProgressHindrance?.Select(h => new DailyProgressHindranceModel(
            h.ID,
            h.UniqueID,
            h.Hindrance,
            h.AudioUrl)).ToArray() ?? Array.Empty<DailyProgressHindranceModel>();

        var photos = d.DailyProgressPhoto?.Select(p => new DailyProgressPhotoModel(
            p.ID,
            p.UniqueID,
            p.PhotoUrl,
            p.Caption)).ToArray() ?? Array.Empty<DailyProgressPhotoModel>();

        return new DailyProgressModel(d.ID, d.UniqueID, d.ProjectID, d.ReportDate, d.NextDayPlan, d.Remarks, d.TotalAmount, d.StatusID, d.IsActive, d.CreatedBy, d.CreatedDate, d.LastModifiedBy, d.LastModifiedDate, details, hindrances, photos);
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
            NextDayPlan = r.NextDayPlan,
            Remarks = r.Remarks,
            TotalAmount = 0m,
            StatusID = r.StatusID,
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
                var detail = new DailyProgressDetail
                {
                    UniqueID = Guid.NewGuid(),
                    ActivityID = d.ActivityId,
                    Quantity = d.Quantity,
                    UOMID = d.UOMID,
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

        if (r.Hindrances?.Any() == true)
        {
            foreach (var h in r.Hindrances)
            {
                var hindrance = new DailyProgressHindrance
                {
                    UniqueID = Guid.NewGuid(),
                    Hindrance = h.Hindrance,
                    AudioUrl = h.AudioUrl,
                    IsActive = true,
                    CreatedBy = userId,
                    CreatedDate = DateTimeOffset.UtcNow,
                    LastModifiedBy = userId,
                    LastModifiedDate = DateTimeOffset.UtcNow
                };

                entity.DailyProgressHindrance?.Add(hindrance);
            }
        }

        if (r.Photos?.Any() == true)
        {
            foreach (var p in r.Photos)
            {
                var photo = new DailyProgressPhoto
                {
                    UniqueID = Guid.NewGuid(),
                    PhotoUrl = p.PhotoUrl,
                    Caption = p.Caption,
                    IsActive = true,
                    CreatedBy = userId,
                    CreatedDate = DateTimeOffset.UtcNow,
                    LastModifiedBy = userId,
                    LastModifiedDate = DateTimeOffset.UtcNow
                };

                entity.DailyProgressPhoto?.Add(photo);
            }
        }

        _db.Set<Himapp.Execution.Domain.Entities.DailyProgress>().Add(entity);
        await _db.SaveChangesAsync(cancellationToken);

        var details = entity.DailyProgressDetail?.Select(dd => new DailyProgressDetailModel(dd.ID, dd.UniqueID, dd.ActivityID, dd.Quantity, dd.UOMID, dd.Rate, dd.Amount, dd.PlanQuantity, dd.Variance, dd.Remarks)).ToArray() ?? Array.Empty<DailyProgressDetailModel>();
        var hindrances = entity.DailyProgressHindrance?.Select(h => new DailyProgressHindranceModel(
            h.ID,
            h.UniqueID,
            h.Hindrance,
            h.AudioUrl)).ToArray() ?? Array.Empty<DailyProgressHindranceModel>();
        var photos = entity.DailyProgressPhoto?.Select(p => new DailyProgressPhotoModel(
            p.ID,
            p.UniqueID,
            p.PhotoUrl,
            p.Caption)).ToArray() ?? Array.Empty<DailyProgressPhotoModel>();

        return new DailyProgressModel(entity.ID, entity.UniqueID, entity.ProjectID, entity.ReportDate, entity.NextDayPlan, entity.Remarks, entity.TotalAmount, entity.StatusID, entity.IsActive, entity.CreatedBy, entity.CreatedDate, entity.LastModifiedBy, entity.LastModifiedDate, details, hindrances, photos);
    }

    public async Task<DailyProgressModel?> Handle(UpdateDailyProgressCommand request, CancellationToken cancellationToken)
    {
        var userId = CurrentUserId;
        var entity = await _db.Set<Himapp.Execution.Domain.Entities.DailyProgress>()
            .Include(d => d.DailyProgressDetail)
            .Include(d => d.DailyProgressHindrance)
            .Include(d => d.DailyProgressPhoto)
            .FirstOrDefaultAsync(x => x.ID == request.Id, cancellationToken);
        if (entity is null) return null;

        int LastModifiedBy = request.Request.LastModifiedBy;

        entity.NextDayPlan = request.Request.NextDayPlan ?? entity.NextDayPlan;
        entity.Remarks = request.Request.Remarks ?? entity.Remarks;
        entity.StatusID = request.Request.StatusID;
        entity.LastModifiedBy = LastModifiedBy;
        entity.LastModifiedDate = DateTimeOffset.UtcNow;

        // Remove existing details and add new ones
        if (entity.DailyProgressDetail != null && entity.DailyProgressDetail.Any())
        {
            _db.Set<DailyProgressDetail>().RemoveRange(entity.DailyProgressDetail);
            entity.DailyProgressDetail.Clear();
        }

        if (request.Request.Details?.Any() == true)
        {
            foreach (var d in request.Request.Details)
            {
                var detail = new DailyProgressDetail
                {
                    UniqueID = Guid.NewGuid(),
                    ActivityID = d.ActivityId,
                    Quantity = d.Quantity,
                    UOMID = d.UOMID,
                    Rate = d.Rate,
                    PlanQuantity = d.PlanQuantity,
                    Remarks = d.Remarks,
                    IsActive = true,
                    CreatedBy = LastModifiedBy,
                    CreatedDate = DateTimeOffset.UtcNow,
                    LastModifiedBy = LastModifiedBy,
                    LastModifiedDate = DateTimeOffset.UtcNow
                };

                entity.DailyProgressDetail?.Add(detail);
            }
        }

        // Remove existing hindrances and add new ones
        if (entity.DailyProgressHindrance != null && entity.DailyProgressHindrance.Any())
        {
            _db.Set<DailyProgressHindrance>().RemoveRange(entity.DailyProgressHindrance);
            entity.DailyProgressHindrance.Clear();
        }

        if (request.Request.Hindrances?.Any() == true)
        {
            foreach (var h in request.Request.Hindrances)
            {
                var hindrance = new DailyProgressHindrance
                {
                    UniqueID = Guid.NewGuid(),
                    Hindrance = h.Hindrance,
                    AudioUrl = h.AudioUrl,
                    IsActive = true,
                    CreatedBy = LastModifiedBy,
                    CreatedDate = DateTimeOffset.UtcNow,
                    LastModifiedBy = LastModifiedBy,
                    LastModifiedDate = DateTimeOffset.UtcNow
                };

                entity.DailyProgressHindrance?.Add(hindrance);
            }
        }

        // Remove existing photos and add new ones
        if (entity.DailyProgressPhoto != null && entity.DailyProgressPhoto.Any())
        {
            _db.Set<DailyProgressPhoto>().RemoveRange(entity.DailyProgressPhoto);
            entity.DailyProgressPhoto.Clear();
        }

        if (request.Request.Photos?.Any() == true)
        {
            foreach (var p in request.Request.Photos)
            {
                var photo = new DailyProgressPhoto
                {
                    UniqueID = Guid.NewGuid(),
                    PhotoUrl = p.PhotoUrl,
                    Caption = p.Caption,
                    IsActive = true,
                    CreatedBy = LastModifiedBy,
                    CreatedDate = DateTimeOffset.UtcNow,
                    LastModifiedBy = LastModifiedBy,
                    LastModifiedDate = DateTimeOffset.UtcNow
                };

                entity.DailyProgressPhoto?.Add(photo);
            }
        }

        await _db.SaveChangesAsync(cancellationToken);

        var details = entity.DailyProgressDetail?.Select(dd => new DailyProgressDetailModel(dd.ID, dd.UniqueID, dd.ActivityID, dd.Quantity, dd.UOMID, dd.Rate, dd.Amount, dd.PlanQuantity, dd.Variance, dd.Remarks)).ToArray() ?? Array.Empty<DailyProgressDetailModel>();
        var hindrances = entity.DailyProgressHindrance?.Select(h => new DailyProgressHindranceModel(
            h.ID,
            h.UniqueID,
            h.Hindrance,
            h.AudioUrl)).ToArray() ?? Array.Empty<DailyProgressHindranceModel>();
        var photos = entity.DailyProgressPhoto?.Select(p => new DailyProgressPhotoModel(
            p.ID,
            p.UniqueID,
            p.PhotoUrl,
            p.Caption)).ToArray() ?? Array.Empty<DailyProgressPhotoModel>();

        return new DailyProgressModel(entity.ID, entity.UniqueID, entity.ProjectID, entity.ReportDate, entity.NextDayPlan, entity.Remarks, entity.TotalAmount, entity.StatusID, entity.IsActive, entity.CreatedBy, entity.CreatedDate, entity.LastModifiedBy, entity.LastModifiedDate, details, hindrances, photos);
    }

    public async Task<bool> Handle(DeleteDailyProgressCommand request, CancellationToken cancellationToken)
    {
        var userId = CurrentUserId;
        var entity = await _db.Set<Himapp.Execution.Domain.Entities.DailyProgress>()
            .Include(d => d.DailyProgressDetail)
            .Include(d => d.DailyProgressHindrance)
            .Include(d => d.DailyProgressPhoto)
            .FirstOrDefaultAsync(x => x.ID == request.dtoInactive.ProgramRowId, cancellationToken);

        if (entity is null) return false;

        bool isActive = request.dtoInactive.Actions == Actions.Activated;

        // Soft delete header and child details
        entity.IsActive = isActive;
        entity.LastModifiedBy = userId;
        entity.LastModifiedDate = DateTimeOffset.UtcNow;

        if (entity.DailyProgressDetail != null)
        {
            foreach (var dd in entity.DailyProgressDetail)
            {
                dd.IsActive = isActive;
                dd.LastModifiedBy = userId;
                dd.LastModifiedDate = DateTimeOffset.UtcNow;
            }
        }

        if (entity.DailyProgressHindrance != null)
        {
            foreach (var h in entity.DailyProgressHindrance)
            {
                h.IsActive = isActive;
                h.LastModifiedBy = userId;
                h.LastModifiedDate = DateTimeOffset.UtcNow;
            }
        }

        if (entity.DailyProgressPhoto != null)
        {
            foreach (var p in entity.DailyProgressPhoto)
            {
                p.IsActive = isActive;
                p.LastModifiedBy = userId;
                p.LastModifiedDate = DateTimeOffset.UtcNow;
            }
        }

        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<DataSet> Handle(GetDailyProgressListByProjectQuery request, CancellationToken cancellationToken)
    {
        DataSet ds = new DataSet();

        string connectionString =
            _db.Database.GetDbConnection().ConnectionString;

        #region LIST

        using (var connection = new NpgsqlConnection(connectionString))
        {
            DataTable dt = new DataTable();

            await connection.OpenAsync(cancellationToken);

            using var cmd = new NpgsqlCommand(
                @"SELECT * FROM execution.uspgetprojectdprbyprojectid(
                @p_projectid,
                @p_filtercolumn,
                @p_filtervalue,
                @p_pageindex,
                @p_pagesize,
                @p_sortcolumn,
                @p_isactive)",
                connection);

            cmd.Parameters.AddWithValue(
                "@p_projectid",
                request.SearchParams.ProjectID);

            cmd.Parameters.AddWithValue(
                "@p_filtercolumn",
                request.SearchParams.FilterColumn ??
                (object)DBNull.Value);

            cmd.Parameters.AddWithValue(
                "@p_filtervalue",
                request.SearchParams.FilterValue ??
                (object)DBNull.Value);

            cmd.Parameters.AddWithValue(
                "@p_pageindex",
                request.SearchParams.PageIndex);

            cmd.Parameters.AddWithValue(
                "@p_pagesize",
                request.SearchParams.PageSize);

            cmd.Parameters.AddWithValue(
                "@p_sortcolumn",
                request.SearchParams.SortColumn ??
                (object)DBNull.Value);

            cmd.Parameters.AddWithValue(
                "@p_isactive",
                request.SearchParams.IsActive ?? "true");

            using var da = new NpgsqlDataAdapter(cmd);

            da.Fill(dt);

            ds.Tables.Add(dt);
        }

        #endregion

        #region COUNT

        using (var connection = new NpgsqlConnection(connectionString))
        {
            DataTable dt = new DataTable();

            await connection.OpenAsync(cancellationToken);

            using var cmd = new NpgsqlCommand(
                @"SELECT * FROM execution.uspgetprojectdprcountbyprojectid(
                @p_projectid,
                @p_filtercolumn,
                @p_filtervalue,
                @p_pageindex,
                @p_pagesize,
                @p_sortcolumn,
                @p_isactive)",
                connection);

            cmd.Parameters.AddWithValue(
                "@p_projectid",
                request.SearchParams.ProjectID);

            cmd.Parameters.AddWithValue(
                "@p_filtercolumn",
                request.SearchParams.FilterColumn ??
                (object)DBNull.Value);

            cmd.Parameters.AddWithValue(
                "@p_filtervalue",
                request.SearchParams.FilterValue ??
                (object)DBNull.Value);

            cmd.Parameters.AddWithValue(
                "@p_pageindex",
                request.SearchParams.PageIndex);

            cmd.Parameters.AddWithValue(
                "@p_pagesize",
                request.SearchParams.PageSize);

            cmd.Parameters.AddWithValue(
                "@p_sortcolumn",
                request.SearchParams.SortColumn ??
                (object)DBNull.Value);

            cmd.Parameters.AddWithValue(
                "@p_isactive",
                request.SearchParams.IsActive ?? "true");

            using var da = new NpgsqlDataAdapter(cmd);

            da.Fill(dt);

            ds.Tables.Add(dt);
        }

        #endregion

        return ds;
    }
}
