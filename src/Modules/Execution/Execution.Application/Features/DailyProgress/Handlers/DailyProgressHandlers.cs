using Himapp.Execution.Application.Features.DailyDepartmentalLabourSlip.Handlers;
using Himapp.Execution.Application.Features.DailyProgress.Commands;
using Himapp.Execution.Application.Features.DailyProgress.Models;
using Himapp.Execution.Application.Features.DailyProgress.Queries;
using Himapp.Execution.Application.Features.Planning.Queries;
using Himapp.Execution.Application.Features.SiteDailyProgress.Models;
using Himapp.Execution.Application.Features.SiteDailyProgress.Queries;
using Himapp.Execution.Contracts;
using Himapp.Execution.Contracts.References;
using Himapp.Execution.Domain.Entities;
using Himapp.SharedKernel.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using System.Data;
using DailyProgressEntity = Himapp.Execution.Domain.Entities.DailyProgress;
using PlanningEntity = Himapp.Execution.Domain.Entities.Planning;

namespace Himapp.Execution.Application.Features.DailyProgress.Handlers;

internal sealed class DailyProgressHandlers :
    IRequestHandler<GetAllDailyProgressQuery, IReadOnlyCollection<DailyProgressModel>>,
    IRequestHandler<GetDailyProgressByIdQuery, DailyProgressModel?>,
    IRequestHandler<CreateDailyProgressCommand, DailyProgressModel>,
    IRequestHandler<UpdateDailyProgressCommand, DailyProgressModel?>,
    IRequestHandler<DeleteDailyProgressCommand, bool>,
    IRequestHandler<GetDailyProgressListByProjectQuery, DataSet>,
    IRequestHandler<GetActivityWiseQuantityByProjectQuery, List<ActivityWiseQuantityBySectionModel>>,
    IRequestHandler<GetDailyProgressByProjectAndDateQuery, DailyProgressModel?>
{
    private readonly IExecutionDbContext _db;
    private readonly ICurrentUser _currentUser;
    private readonly IDPRCodeGenerator _codeGenerator;
    private readonly ILogger<DailyProgressHandlers> _logger;
    public DailyProgressHandlers(IExecutionDbContext db, ICurrentUser currentUser, IDPRCodeGenerator codeGenerator, ILogger<DailyProgressHandlers> logger) => (_db, _currentUser, _codeGenerator, _logger) 
        = (db, currentUser, codeGenerator, logger);

    private int CurrentUserId => _currentUser.UserId ?? 0;

    public async Task<IReadOnlyCollection<DailyProgressModel>> Handle(GetAllDailyProgressQuery request, CancellationToken cancellationToken)
    {
        return await _db.Set<DailyProgressEntity>()
            .AsNoTracking()
            .Select(d => new DailyProgressModel(d.ID, d.UniqueID, d.ProjectID, d.DPRCode, d.ReportDate, d.NextDayPlan, d.Remarks, d.TotalAmount, d.StatusID, d.IsActive, d.CreatedBy, d.CreatedDate, d.LastModifiedBy, d.LastModifiedDate, Array.Empty<DailyProgressDetailModel>(), Array.Empty<DailyProgressHindranceModel>(), Array.Empty<DailyProgressPhotoModel>()))
            .ToArrayAsync(cancellationToken);
    }

    public async Task<DailyProgressModel?> Handle(GetDailyProgressByIdQuery request, CancellationToken cancellationToken)
    {
        var d = await _db.Set<DailyProgressEntity>()
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
            p.FileName,
            p.FileType,
            p.FileSize,
            p.PhotoUrl,
            p.Caption)).ToArray() ?? Array.Empty<DailyProgressPhotoModel>();

        return new DailyProgressModel(d.ID, d.UniqueID, d.ProjectID, d.DPRCode, d.ReportDate, d.NextDayPlan, d.Remarks, d.TotalAmount, d.StatusID, d.IsActive, d.CreatedBy, d.CreatedDate, d.LastModifiedBy, d.LastModifiedDate, details, hindrances, photos);
    }

    public async Task<DailyProgressModel> Handle(CreateDailyProgressCommand request, CancellationToken cancellationToken)
    {
        var r = request.Request;
        var userId = CurrentUserId;

        // Generate DPRCode project-wise before constructing the entity so we can log and inspect it
        var generatedCode = await _codeGenerator.GenerateDPRCodeAsync(r.ProjectId, cancellationToken);
        _logger.LogDebug("DPR code generated for ProjectId {ProjectId}: '{Code}'", r.ProjectId, generatedCode);

        var entity = new DailyProgressEntity
        {
            UniqueID = Guid.NewGuid(),
            ProjectID = r.ProjectId,
            ReportDate = r.ReportDate,
            NextDayPlan = r.NextDayPlan,
            Remarks = r.Remarks,
            DPRCode = generatedCode,
            TotalAmount = 0m,
            StatusID = r.StatusID,
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
                    CreatedDate = DateTime.UtcNow,
                    LastModifiedBy = userId,
                    LastModifiedDate = DateTime.UtcNow
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
                    CreatedDate = DateTime.UtcNow,
                    LastModifiedBy = userId,
                    LastModifiedDate = DateTime.UtcNow
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
                    FileName = p.FileName,
                    FileType = p.FileType,
                    FileSize = p.FileSize,
                    PhotoUrl = p.PhotoUrl,
                    Caption = p.Caption,
                    IsActive = true,
                    CreatedBy = userId,
                    CreatedDate = DateTime.UtcNow,
                    LastModifiedBy = userId,
                    LastModifiedDate = DateTime.UtcNow
                };

                entity.DailyProgressPhoto?.Add(photo);
            }
        }

        _db.Set<DailyProgressEntity>().Add(entity);
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
            p.FileName,
            p.FileType,
            p.FileSize,
            p.PhotoUrl,
            p.Caption)).ToArray() ?? Array.Empty<DailyProgressPhotoModel>();

        return new DailyProgressModel(entity.ID, entity.UniqueID, entity.ProjectID, entity.DPRCode, entity.ReportDate, entity.NextDayPlan, entity.Remarks, entity.TotalAmount, entity.StatusID, entity.IsActive, entity.CreatedBy, entity.CreatedDate, entity.LastModifiedBy, entity.LastModifiedDate, details, hindrances, photos);
    }

    public async Task<DailyProgressModel?> Handle(UpdateDailyProgressCommand request, CancellationToken cancellationToken)
    {
        var userId = CurrentUserId;
        var entity = await _db.Set<DailyProgressEntity>()
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
        entity.LastModifiedDate = DateTime.UtcNow;

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
                    CreatedDate = DateTime.UtcNow,
                    LastModifiedBy = LastModifiedBy,
                    LastModifiedDate = DateTime.UtcNow
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
                    CreatedDate = DateTime.UtcNow,
                    LastModifiedBy = LastModifiedBy,
                    LastModifiedDate = DateTime.UtcNow
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
                    FileName = p.FileName,
                    FileType = p.FileType,
                    FileSize = p.FileSize,
                    Caption = p.Caption,
                    IsActive = true,
                    CreatedBy = LastModifiedBy,
                    CreatedDate = DateTime.UtcNow,
                    LastModifiedBy = LastModifiedBy,
                    LastModifiedDate = DateTime.UtcNow
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
            p.FileName,
            p.FileType,
            p.FileSize,
            p.PhotoUrl,
            p.Caption)).ToArray() ?? Array.Empty<DailyProgressPhotoModel>();

        return new DailyProgressModel(entity.ID, entity.UniqueID, entity.ProjectID, entity.DPRCode, entity.ReportDate, entity.NextDayPlan, entity.Remarks, entity.TotalAmount, entity.StatusID, entity.IsActive, entity.CreatedBy, entity.CreatedDate, entity.LastModifiedBy, entity.LastModifiedDate, details, hindrances, photos);
    }

    public async Task<bool> Handle(DeleteDailyProgressCommand request, CancellationToken cancellationToken)
    {
        var userId = CurrentUserId;
        var entity = await _db.Set<DailyProgressEntity>()
            .Include(d => d.DailyProgressDetail)
            .Include(d => d.DailyProgressHindrance)
            .Include(d => d.DailyProgressPhoto)
            .FirstOrDefaultAsync(x => x.ID == request.dtoInactive.ProgramRowId, cancellationToken);

        if (entity is null) return false;

        bool isActive = request.dtoInactive.Actions == Actions.Activated;

        // Soft delete header and child details
        entity.IsActive = isActive;
        entity.LastModifiedBy = userId;
        entity.LastModifiedDate = DateTime.UtcNow;

        if (entity.DailyProgressDetail != null)
        {
            foreach (var dd in entity.DailyProgressDetail)
            {
                dd.IsActive = isActive;
                dd.LastModifiedBy = userId;
                dd.LastModifiedDate = DateTime.UtcNow;
            }
        }

        if (entity.DailyProgressHindrance != null)
        {
            foreach (var h in entity.DailyProgressHindrance)
            {
                h.IsActive = isActive;
                h.LastModifiedBy = userId;
                h.LastModifiedDate = DateTime.UtcNow;
            }
        }

        if (entity.DailyProgressPhoto != null)
        {
            foreach (var p in entity.DailyProgressPhoto)
            {
                p.IsActive = isActive;
                p.LastModifiedBy = userId;
                p.LastModifiedDate = DateTime.UtcNow;
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

    public async Task<List<ActivityWiseQuantityBySectionModel>> Handle(GetActivityWiseQuantityByProjectQuery request, CancellationToken cancellationToken)
    {
        var dbContext = _db as DbContext;

        if (dbContext is null)
        {
            throw new InvalidOperationException("IExecutionDbContext is not a DbContext.");
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

                LEFT JOIN public."UnitOfMeasurement" uom
                    ON pd."UOMID" = uom."ID"

                WHERE
                    p."ProjectID" = @ProjectID
                    AND @ReportDate BETWEEN p."StartDate" AND p."EndDate"
                    AND p."IsActive" = TRUE
                    AND pd."IsActive" = TRUE

                ORDER BY
                    activity."ActivityName";
                """;

            var projectIdParameter = command.CreateParameter();
            projectIdParameter.ParameterName = "@ProjectID";
            projectIdParameter.Value = request.ProjectID;
            command.Parameters.Add(projectIdParameter);

            var reportDateParameter = command.CreateParameter();
            reportDateParameter.ParameterName = "@ReportDate";
            reportDateParameter.Value = request.ReportDate;
            command.Parameters.Add(reportDateParameter);

            using var reader = await command.ExecuteReaderAsync(cancellationToken);

            var activityIdOrdinal = reader.GetOrdinal("ActivityID");
            var activityNameOrdinal = reader.GetOrdinal("ActivityName");
            var targetQuantityOrdinal = reader.GetOrdinal("TargetQuantity");
            var uomIdOrdinal = reader.GetOrdinal("UOMID");
            var uomNameOrdinal = reader.GetOrdinal("UOMName");
            var uomShortNameOrdinal = reader.GetOrdinal("UOMShortName");
            var revenueRate = reader.GetOrdinal("RevenueRate");

            while (await reader.ReadAsync(cancellationToken))
            {
                result.Add(new ActivityWiseQuantityBySectionModel
                {
                    ActivityID = reader.GetInt32(activityIdOrdinal),

                    ActivityName = reader.IsDBNull(activityNameOrdinal)
                        ? null
                        : reader.GetString(activityNameOrdinal),

                    TargetQuantity = reader.GetDecimal(targetQuantityOrdinal),

                    UOMID = reader.IsDBNull(uomIdOrdinal)
                        ? 0
                        : reader.GetInt32(uomIdOrdinal),

                    UOMName = reader.IsDBNull(uomNameOrdinal)
                        ? string.Empty
                        : reader.GetString(uomNameOrdinal),

                    UOMShortName = reader.IsDBNull(uomShortNameOrdinal)
                        ? string.Empty
                        : reader.GetString(uomShortNameOrdinal),

                    RevenueRate = reader.IsDBNull(revenueRate)
                        ? 0
                        : reader.GetDecimal(revenueRate)
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

    public async Task<DailyProgressModel?> Handle(GetDailyProgressByProjectAndDateQuery request, CancellationToken cancellationToken)
    {
        var d = await _db.Set<DailyProgressEntity>()
            .AsNoTracking()
            .Include(x => x.DailyProgressDetail)
            .Include(x => x.DailyProgressHindrance)
            .Include(x => x.DailyProgressPhoto)
            .FirstOrDefaultAsync(x => x.ProjectID == request.ProjectId && x.ReportDate == request.ReportDate && x.IsActive, cancellationToken);
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
            p.FileName,
            p.FileType,
            p.FileSize,
            p.PhotoUrl,
            p.Caption)).ToArray() ?? Array.Empty<DailyProgressPhotoModel>();

        return new DailyProgressModel(d.ID, d.UniqueID, d.ProjectID, d.DPRCode, d.ReportDate, d.NextDayPlan, d.Remarks, d.TotalAmount, d.StatusID, d.IsActive, d.CreatedBy, d.CreatedDate, d.LastModifiedBy, d.LastModifiedDate, details, hindrances, photos);
    }
}
