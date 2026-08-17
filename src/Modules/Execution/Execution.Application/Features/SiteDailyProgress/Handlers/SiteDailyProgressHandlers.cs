using Himapp.Execution.Application.Features.DailyLabor.Queries;
using Himapp.Execution.Application.Features.SiteDailyProgress.Commands;
using Himapp.Execution.Application.Features.SiteDailyProgress.Models;
using Himapp.Execution.Application.Features.SiteDailyProgress.Queries;
using Himapp.Execution.Contracts;
using Himapp.Execution.Domain.Entities;
using Himapp.SharedKernel.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using NpgsqlTypes;
using System.Data;

namespace Himapp.Execution.Application.Features.SiteDailyProgress.Handlers;

internal sealed class SiteDailyProgressHandlers :
    IRequestHandler<GetAllSiteDailyProgressesQuery, IEnumerable<SiteDailyProgressModel>>,
    IRequestHandler<GetSiteDailyProgressByIdQuery, SiteDailyProgressModel?>,
    IRequestHandler<CreateSiteDailyProgressCommand, SiteDailyProgressModel>,
    IRequestHandler<UpdateSiteDailyProgressCommand, SiteDailyProgressModel?>,
    IRequestHandler<DeleteSiteDailyProgressCommand, bool>,
    IRequestHandler<GetSiteDailyProgressByProjectIDQuery, DataSet>,
    IRequestHandler<DeleteSiteDPRCommand, bool>
{
    private readonly IExecutionDbContext _db;
    private readonly ICurrentUser _currentUser;
    public SiteDailyProgressHandlers(IExecutionDbContext db, ICurrentUser currentUser) => (_db, _currentUser) = (db, currentUser);

    private int CurrentUserId => _currentUser.UserId ?? 1;

    public async Task<IEnumerable<SiteDailyProgressModel>> Handle(GetAllSiteDailyProgressesQuery request, CancellationToken cancellationToken)
    {
        return await _db.Set<Himapp.Execution.Domain.Entities.SiteDailyProgress>()
            .AsNoTracking()
            .Where(d => d.IsActive)
            .Select(d => new SiteDailyProgressModel(d.ID, d.ProjectID, d.ReportDate, d.Remarks, d.IsActive, d.CreatedBy, d.CreatedDate, d.LastModifiedBy, d.LastModifiedDate, Array.Empty<SiteDailyProgressDetailModel>(),
            Array.Empty<SiteDailyProgressHindranceModel>(), Array.Empty<SiteDailyProgressPhotoModel>()))
            .ToArrayAsync(cancellationToken);
    }

    public async Task<SiteDailyProgressModel?> Handle(GetSiteDailyProgressByIdQuery request, CancellationToken cancellationToken)
    {
        var d = await _db.Set<Himapp.Execution.Domain.Entities.SiteDailyProgress>()
            .AsNoTracking()
            .Include(x => x.SiteDailyProgressDetail)
            .Include(x => x.SiteDailyProgressHindrance)
            .Include(x => x.SiteDailyProgressPhoto)
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

        var hindrances = d.SiteDailyProgressHindrance?.Select(h => new SiteDailyProgressHindranceModel(
            h.ID,
            h.UniqueID,
            h.Hindrance,
            h.AudioUrl)).ToArray() ?? Array.Empty<SiteDailyProgressHindranceModel>();

        var photos = d.SiteDailyProgressPhoto.Where(p => p.IsActive).Select(p => new SiteDailyProgressPhotoModel(p.ID, p.UniqueID, p.PhotoUrl, p.Caption)).ToArray();

        return new SiteDailyProgressModel(d.ID, d.ProjectID, d.ReportDate, d.Remarks, d.IsActive, d.CreatedBy, d.CreatedDate, d.LastModifiedBy, d.LastModifiedDate, details, hindrances, photos);
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
            Remarks = r.Remarks,
            NextDayPlan = null,
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

        if (r.Hindrances?.Any() == true)
        {
            foreach (var h in r.Hindrances)
            {
                var hindrance = new Himapp.Execution.Domain.Entities.SiteDailyProgressHindrance
                {
                    UniqueID = Guid.NewGuid(),
                    Hindrance = h.Hindrance,

                    IsActive = true,
                    CreatedBy = userId,
                    CreatedDate = DateTimeOffset.UtcNow,
                    LastModifiedBy = userId,
                    LastModifiedDate = DateTimeOffset.UtcNow
                };

                entity.SiteDailyProgressHindrance.Add(hindrance);
            }
        }

        if (r.Photos?.Any() == true)
        {
            foreach (var p in r.Photos)
            {
                var photo = new Himapp.Execution.Domain.Entities.SiteDailyProgressPhoto
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

                entity.SiteDailyProgressPhoto.Add(photo);
            }
        }

        _db.Set<Himapp.Execution.Domain.Entities.SiteDailyProgress>().Add(entity);
        await _db.SaveChangesAsync(cancellationToken);



        var details = entity.SiteDailyProgressDetail?.Select(dd => new SiteDailyProgressDetailModel(dd.ID, dd.UniqueId, dd.ActivityID, dd.Quantity, dd.UOMID, dd.Rate, dd.Amount, dd.PlanQuantity, dd.Variance, dd.Remarks)).ToArray()
            ?? Array.Empty<SiteDailyProgressDetailModel>();

        var hindrances = entity.SiteDailyProgressHindrance.Select(h => new SiteDailyProgressHindranceModel(h.ID, h.UniqueID, h.Hindrance, h.AudioUrl)).ToArray();

        var photos = entity.SiteDailyProgressPhoto?.Select(p => new SiteDailyProgressPhotoModel(
            p.ID,
            p.UniqueID,
            p.PhotoUrl,
            p.Caption)).ToArray() ?? Array.Empty<SiteDailyProgressPhotoModel>();

        return new SiteDailyProgressModel(entity.ID, entity.ProjectID, entity.ReportDate, entity.Remarks, entity.IsActive, entity.CreatedBy, entity.CreatedDate, entity.LastModifiedBy, entity.LastModifiedDate, details, hindrances, photos);
    }

    public async Task<SiteDailyProgressModel?> Handle(UpdateSiteDailyProgressCommand request, CancellationToken cancellationToken)
    {
        var userId = CurrentUserId;
        var entity = await _db.Set<Himapp.Execution.Domain.Entities.SiteDailyProgress>()
            .Include(d => d.SiteDailyProgressDetail)
            .Include(d => d.SiteDailyProgressHindrance)
            .Include(d => d.SiteDailyProgressPhoto).FirstOrDefaultAsync(x => x.ID == request.Id && x.IsActive, cancellationToken);
        if (entity is null) return null;

        var r = request.Request;

        entity.ProjectID = r.ProjectId;
        entity.ReportDate = r.ReportDate.HasValue ? DateOnly.FromDateTime(r.ReportDate.Value.UtcDateTime) : entity.ReportDate;
        entity.Remarks = r.Remarks ?? entity.Remarks;
        entity.LastModifiedBy = userId;
        entity.LastModifiedDate = DateTimeOffset.UtcNow;

        // ---------------------------------------------------------
        // Remove existing Details
        // ---------------------------------------------------------

        if (entity.SiteDailyProgressDetail != null && entity.SiteDailyProgressDetail.Any())
        {
            _db.Set<Himapp.Execution.Domain.Entities.SiteDailyProgressDetail>()
                .RemoveRange(entity.SiteDailyProgressDetail);

            entity.SiteDailyProgressDetail.Clear();
        }

        // ---------------------------------------------------------
        // Add new Details
        // ---------------------------------------------------------

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

        // ---------------------------------------------------------
        // Remove existing Hindrances
        // ---------------------------------------------------------

        if (entity.SiteDailyProgressHindrance != null && entity.SiteDailyProgressHindrance.Any())
        {
            _db.Set<Himapp.Execution.Domain.Entities.SiteDailyProgressHindrance>().RemoveRange(entity.SiteDailyProgressHindrance);

            entity.SiteDailyProgressHindrance.Clear();
        }

        // ---------------------------------------------------------
        // Add new Hindrances
        // ---------------------------------------------------------

        if (r.Hindrances?.Any() == true)
        {
            foreach (var h in r.Hindrances)
            {
                var hindrance = new Himapp.Execution.Domain.Entities.SiteDailyProgressHindrance
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

                entity.SiteDailyProgressHindrance?.Add(hindrance);
            }
        }

        if (entity.SiteDailyProgressPhoto != null &&
    entity.SiteDailyProgressPhoto.Any())
        {
            _db.Set<Himapp.Execution.Domain.Entities.SiteDailyProgressPhoto>()
                .RemoveRange(entity.SiteDailyProgressPhoto);

            entity.SiteDailyProgressPhoto.Clear();
        }

        if (r.Photos?.Any() == true)
        {
            foreach (var p in r.Photos)
            {
                var photo = new Himapp.Execution.Domain.Entities.SiteDailyProgressPhoto
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

                entity.SiteDailyProgressPhoto?.Add(photo);
            }
        }



        await _db.SaveChangesAsync(cancellationToken);

        var details = entity.SiteDailyProgressDetail?.Select(dd => new SiteDailyProgressDetailModel(dd.ID, dd.UniqueId, dd.ActivityID, dd.Quantity, dd.UOMID, dd.Rate, dd.Amount, dd.PlanQuantity, dd.Variance, dd.Remarks)).ToArray() ?? Array.Empty<SiteDailyProgressDetailModel>();

        var hindrances = entity.SiteDailyProgressHindrance?.Select(h => new SiteDailyProgressHindranceModel(h.ID, h.UniqueID, h.Hindrance, h.AudioUrl)).ToArray() ?? Array.Empty<SiteDailyProgressHindranceModel>();

        var photos = entity.SiteDailyProgressPhoto?.Select(p => new SiteDailyProgressPhotoModel(p.ID, p.UniqueID, p.PhotoUrl, p.Caption)).ToArray() ?? Array.Empty<SiteDailyProgressPhotoModel>();

        return new SiteDailyProgressModel(entity.ID, entity.ProjectID, entity.ReportDate, entity.Remarks, entity.IsActive, entity.CreatedBy, entity.CreatedDate, entity.LastModifiedBy, entity.LastModifiedDate, details, hindrances, photos);
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

    public async Task<bool> Handle(DeleteSiteDPRCommand request, CancellationToken cancellationToken)
    {
        var model = request.addTransactionActionHistoryDTO;
        var entity = await _db.Set<Himapp.Execution.Domain.Entities.SiteDailyProgress>().FirstOrDefaultAsync(a => a.ID == model.ProgramRowId, cancellationToken);

        if (entity is null) return false;

        // Mark child detail records active/inactive
        var details = await _db.Set<Himapp.Execution.Domain.Entities.SiteDailyProgressDetail>()
            .Where(x => x.SiteDailyProgressID == model.ProgramRowId)
            .ToListAsync(cancellationToken);

        foreach (var d in details)
        {
            d.IsActive = model.Actions == Actions.Activated ? true : false;
        }

        // Mark main entity active/inactive
        entity.IsActive = model.Actions == Actions.Activated ? true : false;

        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }
    public async Task<DataSet> Handle(GetSiteDailyProgressByProjectIDQuery request, CancellationToken cancellationToken)
    {
        var p = request.SearchParamsProjectWise ?? new SearchParamsProjectWise();

        // Prepare DataSet
        var ds = new System.Data.DataSet("ActivitiesResult");

        // Force Npgsql path: require the underlying DbContext to obtain connection string
        var dbContext = _db as DbContext;
        if (dbContext is null)
            throw new InvalidOperationException("IExecutionDbContext is not a DbContext. Cannot obtain connection string for Npgsql operations.");

        var dsLocal = new DataSet("ActivitiesResult");
        var connString = dbContext.Database.GetDbConnection().ConnectionString;

        using var conn = new NpgsqlConnection(connString);
        await conn.OpenAsync(cancellationToken);

        // Rows table
        using (var cmd = new NpgsqlCommand("SELECT * FROM execution.uspgetsitedprbyprojectid(@p_projectid,@p_filtercolumn,@p_filtervalue,@p_pageindex,@p_pagesize,@p_sortcolumn,@p_isactive)", conn))
        {
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = 30;
            cmd.Parameters.AddWithValue("@p_projectid", NpgsqlDbType.Integer, p.ProjectID);
            cmd.Parameters.AddWithValue("@p_filtercolumn", NpgsqlDbType.Text, string.IsNullOrWhiteSpace(p.FilterColumn) ? (object)DBNull.Value : p.FilterColumn);
            cmd.Parameters.AddWithValue("@p_filtervalue", NpgsqlDbType.Text, string.IsNullOrWhiteSpace(p.FilterValue) ? (object)DBNull.Value : p.FilterValue);
            cmd.Parameters.AddWithValue("@p_pageindex", NpgsqlDbType.Integer, p.PageIndex);
            cmd.Parameters.AddWithValue("@p_pagesize", NpgsqlDbType.Integer, p.PageSize);
            cmd.Parameters.AddWithValue("@p_sortcolumn", NpgsqlDbType.Text, string.IsNullOrWhiteSpace(p.SortColumn) ? (object)DBNull.Value : p.SortColumn);
            cmd.Parameters.AddWithValue("@p_isactive", NpgsqlDbType.Text, string.IsNullOrWhiteSpace(p.IsActive) ? (object)DBNull.Value : p.IsActive);

            var da = new NpgsqlDataAdapter(cmd);
            var dt = new DataTable("Rows");
            da.Fill(dt);
            dsLocal.Tables.Add(dt);
        }

        // Count table
        using (var cmd2 = new NpgsqlCommand("SELECT cnt FROM execution.uspgetsitedprcountbyprojectid(@p_projectid,@p_filtercolumn,@p_filtervalue,@p_pageindex,@p_pagesize,@p_sortcolumn,@p_isactive)", conn))
        {
            cmd2.CommandType = CommandType.Text;
            cmd2.CommandTimeout = 30;
            cmd2.Parameters.AddWithValue("@p_projectid", NpgsqlDbType.Integer, p.ProjectID);
            cmd2.Parameters.AddWithValue("@p_filtercolumn", NpgsqlDbType.Text, string.IsNullOrWhiteSpace(p.FilterColumn) ? (object)DBNull.Value : p.FilterColumn);
            cmd2.Parameters.AddWithValue("@p_filtervalue", NpgsqlDbType.Text, string.IsNullOrWhiteSpace(p.FilterValue) ? (object)DBNull.Value : p.FilterValue);
            cmd2.Parameters.AddWithValue("@p_pageindex", NpgsqlDbType.Integer, p.PageIndex);
            cmd2.Parameters.AddWithValue("@p_pagesize", NpgsqlDbType.Integer, p.PageSize);
            cmd2.Parameters.AddWithValue("@p_sortcolumn", NpgsqlDbType.Text, string.IsNullOrWhiteSpace(p.SortColumn) ? (object)DBNull.Value : p.SortColumn);
            cmd2.Parameters.AddWithValue("@p_isactive", NpgsqlDbType.Text, string.IsNullOrWhiteSpace(p.IsActive) ? (object)DBNull.Value : p.IsActive);

            var da2 = new NpgsqlDataAdapter(cmd2);
            var dt2 = new DataTable("Count");
            da2.Fill(dt2);
            dsLocal.Tables.Add(dt2);
        }

        return dsLocal;
    }
}
