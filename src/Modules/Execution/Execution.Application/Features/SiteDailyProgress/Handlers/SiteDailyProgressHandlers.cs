using Himapp.Execution.Application.Features.DailyLabor.Queries;
using Himapp.Execution.Application.Features.SiteDailyProgress.Commands;
using Himapp.Execution.Application.Features.SiteDailyProgress.Models;
using Himapp.Execution.Application.Features.SiteDailyProgress.Queries;
using Himapp.Execution.Application.Lookups;
using Himapp.Execution.Contracts;
using Himapp.Execution.Domain.Entities;
using Himapp.SharedKernel.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using NpgsqlTypes;
using System.Data;
using SiteDailyProgressEntity = Himapp.Execution.Domain.Entities.SiteDailyProgress;

namespace Himapp.Execution.Application.Features.SiteDailyProgress.Handlers;

internal sealed class SiteDailyProgressHandlers :
    IRequestHandler<GetAllSiteDailyProgressesQuery, IEnumerable<SiteDailyProgressModel>>,
    IRequestHandler<GetSiteDailyProgressByIdQuery, SiteDailyProgressModel?>,
    IRequestHandler<CreateSiteDailyProgressCommand, SiteDailyProgressModel>,
    IRequestHandler<UpdateSiteDailyProgressCommand, SiteDailyProgressModel?>,
    IRequestHandler<DeleteSiteDailyProgressCommand, bool>,
    IRequestHandler<GetSiteDailyProgressByProjectIDQuery, DataSet>,
    IRequestHandler<DeleteSiteDPRCommand, bool>,
    IRequestHandler<GetActivityWiseQuantityBySectionIDQuery, List<ActivityWiseQuantityBySectionModel>>,
    IRequestHandler<GetLastSiteDPRBySectionIDQuery, SiteDailyProgressModel?>
{
    private readonly IExecutionDbContext _db;
    private readonly ICurrentUser _currentUser;

    public SiteDailyProgressHandlers(IExecutionDbContext db, ICurrentUser currentUser) => (_db, _currentUser) = (db, currentUser);

    private int CurrentUserId => _currentUser.UserId ?? 1;

    public async Task<IEnumerable<SiteDailyProgressModel>> Handle(GetAllSiteDailyProgressesQuery request, CancellationToken cancellationToken)
    {
        return await _db.Set<SiteDailyProgressEntity>()
            .AsNoTracking()
            .Where(d => d.IsActive)
            .Select(d => new SiteDailyProgressModel(d.ID, d.ProjectID, d.ReportDate, d.Remarks, d.IsActive, d.CreatedBy, d.CreatedDate, d.LastModifiedBy, d.LastModifiedDate, d.SectionID, d.NextDayPlan,
            d.TotalAmount, Array.Empty<SiteDailyProgressDetailModel>(),
            Array.Empty<SiteDailyProgressHindranceModel>(), Array.Empty<SiteDailyProgressPhotoModel>()))
            .ToArrayAsync(cancellationToken);
    }

    public async Task<SiteDailyProgressModel?> Handle(GetSiteDailyProgressByIdQuery request, CancellationToken cancellationToken)
    {
        var d = await _db.Set<SiteDailyProgressEntity>()
            .AsNoTracking()
            .Include(x => x.SiteDailyProgressDetail)
            .Include(x => x.SiteDailyProgressHindrance)
            .Include(x => x.SiteDailyProgressPhoto)
            .FirstOrDefaultAsync(x => x.ID == request.Id && x.IsActive, cancellationToken);
        if (d is null) return null;

        var details = d.SiteDailyProgressDetail?.Select(dd => new SiteDailyProgressDetailModel(
            dd.ID,
            dd.UniqueID,
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

        var photos = d.SiteDailyProgressPhoto.Where(p => p.IsActive).Select(p => new SiteDailyProgressPhotoModel(p.ID, p.UniqueID, p.FileName, p.FileType, p.FileSize, p.PhotoUrl, p.Caption)).ToArray();

        return new SiteDailyProgressModel(d.ID, d.ProjectID, d.ReportDate, d.Remarks, d.IsActive, d.CreatedBy, d.CreatedDate, d.LastModifiedBy, d.LastModifiedDate, d.SectionID, d.NextDayPlan, d.TotalAmount, details, hindrances, photos);
    }

    public async Task<SiteDailyProgressModel> Handle(CreateSiteDailyProgressCommand request, CancellationToken cancellationToken)
    {
        var r = request.Request;
        var userId = CurrentUserId;

        var entity = new SiteDailyProgressEntity
        {
            UniqueID = Guid.NewGuid(),
            ProjectID = r.ProjectId,
            SectionID = r.SectionID,
            ReportDate = r.ReportDate.HasValue ? DateOnly.FromDateTime(r.ReportDate.Value.DateTime) : DateOnly.FromDateTime(DateTime.UtcNow),
            Remarks = r.Remarks,
            NextDayPlan = r.NextDayPlan,
            TotalAmount = r.TotalAmount,
            IsActive = true,
            CreatedBy = userId,
            CreatedDate = DateTime.UtcNow,
            LastModifiedBy = userId,
            LastModifiedDate = DateTime.UtcNow
        };

        if (r.Details?.Any() == true)
        {
            foreach (var d in r.Details)
            {
                var detail = new SiteDailyProgressDetail
                {
                    UniqueID = Guid.NewGuid(),
                    ActivityID = d.ActivityId,
                    Quantity = d.Quantity,
                    UOMID = d.UomId,
                    Rate = d.Rate,
                    PlanQuantity = d.PlanQuantity,
                    Remarks = d.Remarks,
                    IsActive = true,
                    CreatedBy = userId,
                    CreatedDate = DateTime.UtcNow,
                    LastModifiedBy = userId,
                    LastModifiedDate = DateTime.UtcNow
                };

                entity.SiteDailyProgressDetail?.Add(detail);
            }
        }

        if (r.Hindrances?.Any() == true)
        {
            foreach (var h in r.Hindrances)
            {
                var hindrance = new SiteDailyProgressHindrance
                {
                    UniqueID = Guid.NewGuid(),
                    Hindrance = h.Hindrance,

                    IsActive = true,
                    CreatedBy = userId,
                    CreatedDate = DateTime.UtcNow,
                    LastModifiedBy = userId,
                    LastModifiedDate = DateTime.UtcNow
                };

                entity.SiteDailyProgressHindrance.Add(hindrance);
            }
        }

        if (r.Photos?.Any() == true)
        {
            foreach (var p in r.Photos)
            {
                var photo = new SiteDailyProgressPhoto
                {
                    UniqueID = Guid.NewGuid(),
                    PhotoUrl = p.PhotoUrl,
                    Caption = p.Caption,
                    FileName = p.FileName,
                    FileType = p.FileType,
                    FileSize = p.FileSize,
                    IsActive = true,
                    CreatedBy = userId,
                    CreatedDate = DateTime.UtcNow,
                    LastModifiedBy = userId,
                    LastModifiedDate = DateTime.UtcNow
                };

                entity.SiteDailyProgressPhoto.Add(photo);
            }
        }

        _db.Set<SiteDailyProgressEntity>().Add(entity);
        await _db.SaveChangesAsync(cancellationToken);



        var details = entity.SiteDailyProgressDetail?.Select(dd => new SiteDailyProgressDetailModel(dd.ID, dd.UniqueID, dd.ActivityID, dd.Quantity, dd.UOMID, dd.Rate, dd.Amount, dd.PlanQuantity, dd.Variance, dd.Remarks)).ToArray()
            ?? Array.Empty<SiteDailyProgressDetailModel>();

        var hindrances = entity.SiteDailyProgressHindrance.Select(h => new SiteDailyProgressHindranceModel(h.ID, h.UniqueID, h.Hindrance, h.AudioUrl)).ToArray();

        var photos = entity.SiteDailyProgressPhoto?.Select(p => new SiteDailyProgressPhotoModel(
            p.ID,
            p.UniqueID,
            p.FileName,
            p.FileType,
            p.FileSize,
            p.PhotoUrl,
            p.Caption)).ToArray() ?? Array.Empty<SiteDailyProgressPhotoModel>();

        return new SiteDailyProgressModel(entity.ID, entity.ProjectID, entity.ReportDate, entity.Remarks, entity.IsActive, entity.CreatedBy, entity.CreatedDate, entity.LastModifiedBy, entity.LastModifiedDate, entity.SectionID,
            r.NextDayPlan, r.TotalAmount, details, hindrances, photos);
    }

    public async Task<SiteDailyProgressModel?> Handle(UpdateSiteDailyProgressCommand request, CancellationToken cancellationToken)
    {
        var userId = CurrentUserId;
        var entity = await _db.Set<SiteDailyProgressEntity>()
            .Include(d => d.SiteDailyProgressDetail)
            .Include(d => d.SiteDailyProgressHindrance)
            .Include(d => d.SiteDailyProgressPhoto).FirstOrDefaultAsync(x => x.ID == request.Id && x.IsActive, cancellationToken);
        if (entity is null) return null;

        var r = request.Request;

        entity.ProjectID = r.ProjectId;
        entity.ReportDate = r.ReportDate ?? entity.ReportDate;
        entity.Remarks = r.Remarks ?? entity.Remarks;
        entity.SectionID = r.SectionID ?? entity.SectionID;
        entity.NextDayPlan = r.NextDayPlan ?? entity.NextDayPlan;
        entity.TotalAmount = r.TotalAmount;
        entity.LastModifiedBy = userId;
        entity.LastModifiedDate = DateTime.UtcNow;

        // ---------------------------------------------------------
        // Remove existing Details
        // ---------------------------------------------------------

        if (entity.SiteDailyProgressDetail != null && entity.SiteDailyProgressDetail.Any())
        {
            _db.Set<SiteDailyProgressDetail>()
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
                var detail = new SiteDailyProgressDetail
                {
                    UniqueID = Guid.NewGuid(),
                    ActivityID = d.ActivityId,
                    Quantity = d.Quantity,
                    UOMID = d.UomId,
                    Rate = d.Rate,
                    PlanQuantity = d.PlanQuantity,
                    Remarks = d.Remarks,

                    IsActive = true,
                    CreatedBy = userId,
                    CreatedDate = DateTime.UtcNow,
                    LastModifiedBy = userId,
                    LastModifiedDate = DateTime.UtcNow
                };

                entity.SiteDailyProgressDetail?.Add(detail);
            }
        }

        // ---------------------------------------------------------
        // Remove existing Hindrances
        // ---------------------------------------------------------

        if (entity.SiteDailyProgressHindrance != null && entity.SiteDailyProgressHindrance.Any())
        {
            _db.Set<SiteDailyProgressHindrance>().RemoveRange(entity.SiteDailyProgressHindrance);

            entity.SiteDailyProgressHindrance.Clear();
        }

        // ---------------------------------------------------------
        // Add new Hindrances
        // ---------------------------------------------------------

        if (r.Hindrances?.Any() == true)
        {
            foreach (var h in r.Hindrances)
            {
                var hindrance = new SiteDailyProgressHindrance
                {
                    UniqueID = Guid.NewGuid(),
                    Hindrance = h.Hindrance,
                    AudioUrl = h.AudioUrl,
                    IsActive = true,
                    CreatedBy = userId,
                    CreatedDate = DateTime.UtcNow,
                    LastModifiedBy = userId,
                    LastModifiedDate = DateTime.UtcNow
                };

                entity.SiteDailyProgressHindrance?.Add(hindrance);
            }
        }

        if (entity.SiteDailyProgressPhoto != null &&
    entity.SiteDailyProgressPhoto.Any())
        {
            _db.Set<SiteDailyProgressPhoto>()
                .RemoveRange(entity.SiteDailyProgressPhoto);

            entity.SiteDailyProgressPhoto.Clear();
        }

        if (r.Photos?.Any() == true)
        {
            foreach (var p in r.Photos)
            {
                var photo = new SiteDailyProgressPhoto
                {
                    UniqueID = Guid.NewGuid(),
                    PhotoUrl = p.PhotoUrl,
                    Caption = p.Caption,
                    FileName = p.FileName,
                    FileType = p.FileType,
                    FileSize = p.FileSize,
                    IsActive = true,
                    CreatedBy = userId,
                    CreatedDate = DateTime.UtcNow,
                    LastModifiedBy = userId,
                    LastModifiedDate = DateTime.UtcNow
                };

                entity.SiteDailyProgressPhoto?.Add(photo);
            }
        }



        await _db.SaveChangesAsync(cancellationToken);

        var details = entity.SiteDailyProgressDetail?.Select(dd => new SiteDailyProgressDetailModel(dd.ID, dd.UniqueID, dd.ActivityID, dd.Quantity, dd.UOMID, dd.Rate, dd.Amount, dd.PlanQuantity, dd.Variance, dd.Remarks)).ToArray() ?? Array.Empty<SiteDailyProgressDetailModel>();

        var hindrances = entity.SiteDailyProgressHindrance?.Select(h => new SiteDailyProgressHindranceModel(h.ID, h.UniqueID, h.Hindrance, h.AudioUrl)).ToArray() ?? Array.Empty<SiteDailyProgressHindranceModel>();

        var photos = entity.SiteDailyProgressPhoto?.Select(p => new SiteDailyProgressPhotoModel(p.ID, p.UniqueID, p.FileName, p.FileType, p.FileSize, p.PhotoUrl, p.Caption)).ToArray() ?? Array.Empty<SiteDailyProgressPhotoModel>();

        return new SiteDailyProgressModel(entity.ID, entity.ProjectID, entity.ReportDate, entity.Remarks, entity.IsActive, entity.CreatedBy, entity.CreatedDate, entity.LastModifiedBy, entity.LastModifiedDate, entity.SectionID,
            entity.NextDayPlan, entity.TotalAmount, details, hindrances, photos);
    }

    public async Task<bool> Handle(DeleteSiteDailyProgressCommand request, CancellationToken cancellationToken)
    {
        var userId = CurrentUserId;
        var entity = await _db.Set<SiteDailyProgressEntity>()
            .Include(d => d.SiteDailyProgressDetail)
            .FirstOrDefaultAsync(x => x.ID == request.Id && x.IsActive, cancellationToken);
        if (entity is null) return false;

        // Soft delete header and child details
        entity.IsActive = false;
        entity.LastModifiedBy = userId;
        entity.LastModifiedDate = DateTime.UtcNow;

        if (entity.SiteDailyProgressDetail != null)
        {
            foreach (var dd in entity.SiteDailyProgressDetail)
            {
                dd.IsActive = false;
                dd.LastModifiedBy = userId;
                dd.LastModifiedDate = DateTime.UtcNow;
            }
        }

        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> Handle(DeleteSiteDPRCommand request, CancellationToken cancellationToken)
    {
        var model = request.addTransactionActionHistoryDTO;
        var entity = await _db.Set<SiteDailyProgressEntity>().FirstOrDefaultAsync(a => a.ID == model.ProgramRowId, cancellationToken);

        if (entity is null) return false;

        // Mark child detail records active/inactive
        var details = await _db.Set<SiteDailyProgressDetail>()
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
            cmd2.CommandTimeout = 10;
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

    public async Task<List<ActivityWiseQuantityBySectionModel>> Handle(GetActivityWiseQuantityBySectionIDQuery request, CancellationToken cancellationToken)
    {
        var dbContext = _db as DbContext;

        if (dbContext is null)
        {
            throw new InvalidOperationException(
                "IExecutionDbContext is not a DbContext.");
        }

        var connection = dbContext.Database.GetDbConnection();

        var result = new List<ActivityWiseQuantityBySectionModel>();

        if (connection.State != System.Data.ConnectionState.Open)
        {
            await connection.OpenAsync(cancellationToken);
        }

        try
        {
            using var command = connection.CreateCommand();

            command.CommandText = """
            SELECT
                pd."ActivityID" AS "ActivityID",
                activity."ActivityName" AS "ActivityName",
                pd."TargetQuantity" AS "TargetQuantity",
                pd."UOMID" AS "UOMID",
                uom."UOMName" AS "UOMName",
                uom."UOMShortName" AS "UOMShortName",
                am."RevenueRate" AS "RevenueRate"

            FROM execution."Plannings" p

            INNER JOIN execution."PlanningDetails" pd
                ON p."ID" = pd."PlanningID"

            INNER JOIN execution."Activities" activity
                ON pd."ActivityID" = activity."ID"

            INNER JOIN execution."ProjectActivities" am
                ON activity."ID" = am."ActivityID"
                AND am."ProjectID" = p."ProjectID"

            LEFT JOIN public."UnitOfMeasurement" uom
                ON pd."UOMID" = uom."ID"

            WHERE
                p."ProjectID" = @ProjectID
                AND pd."AreaID" = @AreaID
                AND @ReportDate BETWEEN p."StartDate" AND p."EndDate"
                AND p."IsActive" = TRUE
                AND pd."IsActive" = TRUE

            ORDER BY
                activity."ActivityName";
            """;

            // ProjectID
            var projectIdParameter = command.CreateParameter();
            projectIdParameter.ParameterName = "@ProjectID";
            projectIdParameter.Value = request.ProjectID;
            command.Parameters.Add(projectIdParameter);

            // AreaID
            var areaIdParameter = command.CreateParameter();
            areaIdParameter.ParameterName = "@AreaID";
            areaIdParameter.Value = request.AreaID;
            command.Parameters.Add(areaIdParameter);

            // ReportDate
            var reportDateParameter = command.CreateParameter();
            reportDateParameter.ParameterName = "@ReportDate";
            reportDateParameter.Value = request.ReportDate;
            command.Parameters.Add(reportDateParameter);

            using var reader = await command.ExecuteReaderAsync(
                cancellationToken);

            var activityIdOrdinal =
                reader.GetOrdinal("ActivityID");

            var activityNameOrdinal =
                reader.GetOrdinal("ActivityName");

            var targetQuantityOrdinal =
                reader.GetOrdinal("TargetQuantity");

            var uomIdOrdinal =
                reader.GetOrdinal("UOMID");

            var uomNameOrdinal =
                reader.GetOrdinal("UOMName");

            var uomShortNameOrdinal =
                reader.GetOrdinal("UOMShortName");

            var revenueRateOrdinal =
                reader.GetOrdinal("RevenueRate");

            while (await reader.ReadAsync(cancellationToken))
            {
                result.Add(new ActivityWiseQuantityBySectionModel
                {
                    ActivityID = reader.GetInt32(activityIdOrdinal),

                    ActivityName = reader.IsDBNull(activityNameOrdinal)
                        ? null
                        : reader.GetString(activityNameOrdinal),

                    TargetQuantity = reader.IsDBNull(targetQuantityOrdinal)
                        ? 0
                        : reader.GetDecimal(targetQuantityOrdinal),

                    UOMID = reader.IsDBNull(uomIdOrdinal)
                        ? 0
                        : reader.GetInt32(uomIdOrdinal),

                    UOMName = reader.IsDBNull(uomNameOrdinal)
                        ? string.Empty
                        : reader.GetString(uomNameOrdinal),

                    UOMShortName = reader.IsDBNull(uomShortNameOrdinal)
                        ? string.Empty
                        : reader.GetString(uomShortNameOrdinal),

                    RevenueRate = reader.IsDBNull(revenueRateOrdinal)
                        ? 0
                        : reader.GetDecimal(revenueRateOrdinal)
                });
            }
        }
        finally
        {
            if (connection.State == System.Data.ConnectionState.Open)
            {
                await connection.CloseAsync();
            }
        }

        return result;
    }

    public async Task<SiteDailyProgressModel?> Handle(GetLastSiteDPRBySectionIDQuery request, CancellationToken cancellationToken)
    {
        var siteDpr = await _db
            .Set<Domain.Entities.SiteDailyProgress>()
            .AsNoTracking()
            .Where(x =>
                x.ProjectID == request.ProjectId &&
                x.SectionID == request.SectionId &&
                x.IsActive)
            .OrderByDescending(x => x.ReportDate)
            .ThenByDescending(x => x.ID)
            .FirstOrDefaultAsync(cancellationToken);

        if (siteDpr == null)
            return null;

        return new SiteDailyProgressModel(
            siteDpr.ID,
            0,
            siteDpr.ReportDate,
            siteDpr.Remarks,
            siteDpr.IsActive,
            siteDpr.CreatedBy,
            siteDpr.CreatedDate,
            siteDpr.LastModifiedBy,
            siteDpr.LastModifiedDate,
            siteDpr.SectionID,
            siteDpr.NextDayPlan,
            siteDpr.TotalAmount,
            // No details required for duplicate-date validation
            Array.Empty<SiteDailyProgressDetailModel>(),

            // No hindrances required
            Array.Empty<SiteDailyProgressHindranceModel>(),

            // No photos required
            Array.Empty<SiteDailyProgressPhotoModel>()
        );
    }
}
