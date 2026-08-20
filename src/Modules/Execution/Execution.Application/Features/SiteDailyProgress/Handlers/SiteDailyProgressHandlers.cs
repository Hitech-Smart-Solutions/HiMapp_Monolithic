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
        return await _db.Set<Himapp.Execution.Domain.Entities.SiteDailyProgress>()
            .AsNoTracking()
            .Where(d => d.IsActive)
            .Select(d => new SiteDailyProgressModel(d.ID, d.ProjectID, d.ReportDate, d.Remarks, d.IsActive, d.CreatedBy, d.CreatedDate, d.LastModifiedBy, d.LastModifiedDate, d.SectionID, d.NextDayPlan, Array.Empty<SiteDailyProgressDetailModel>(),
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

        var photos = d.SiteDailyProgressPhoto.Where(p => p.IsActive).Select(p => new SiteDailyProgressPhotoModel(p.ID, p.UniqueID, p.FileName, p.FileType, p.FileSize, p.PhotoUrl, p.Caption)).ToArray();

        return new SiteDailyProgressModel(d.ID, d.ProjectID, d.ReportDate, d.Remarks, d.IsActive, d.CreatedBy, d.CreatedDate, d.LastModifiedBy, d.LastModifiedDate, d.SectionID, d.NextDayPlan, details, hindrances, photos);
    }

    public async Task<SiteDailyProgressModel> Handle(CreateSiteDailyProgressCommand request, CancellationToken cancellationToken)
    {
        var r = request.Request;
        var userId = CurrentUserId;

        var entity = new Himapp.Execution.Domain.Entities.SiteDailyProgress
        {
            UniqueID = Guid.NewGuid(),
            ProjectID = r.ProjectId,
            SectionID = r.SectionID,
            ReportDate = r.ReportDate.HasValue ? DateOnly.FromDateTime(r.ReportDate.Value.DateTime) : DateOnly.FromDateTime(DateTime.UtcNow),
            Remarks = r.Remarks,
            NextDayPlan = r.NextDayPlan,
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
                    FileName = p.FileName,
                    FileType = p.FileType,
                    FileSize = p.FileSize,
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
            p.FileName, 
            p.FileType, 
            p.FileSize,
            p.PhotoUrl,
            p.Caption)).ToArray() ?? Array.Empty<SiteDailyProgressPhotoModel>();

        return new SiteDailyProgressModel(entity.ID, entity.ProjectID, entity.ReportDate, entity.Remarks, entity.IsActive, entity.CreatedBy, entity.CreatedDate, entity.LastModifiedBy, entity.LastModifiedDate, entity.SectionID, r.NextDayPlan, details, hindrances, photos);
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
        entity.ReportDate = r.ReportDate ?? entity.ReportDate;
        entity.Remarks = r.Remarks ?? entity.Remarks;
        entity.SectionID = r.SectionID ?? entity.SectionID;
        entity.NextDayPlan = r.NextDayPlan ?? entity.NextDayPlan;
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

        var photos = entity.SiteDailyProgressPhoto?.Select(p => new SiteDailyProgressPhotoModel(p.ID, p.UniqueID, p.FileName, p.FileType, p.FileSize,p.PhotoUrl, p.Caption)).ToArray() ?? Array.Empty<SiteDailyProgressPhotoModel>();

        return new SiteDailyProgressModel(entity.ID, entity.ProjectID, entity.ReportDate, entity.Remarks, entity.IsActive, entity.CreatedBy, entity.CreatedDate, entity.LastModifiedBy, entity.LastModifiedDate, entity.SectionID, entity.NextDayPlan, details, hindrances, photos);
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

    public async Task<List<ActivityWiseQuantityBySectionModel>> Handle(GetActivityWiseQuantityBySectionIDQuery request, CancellationToken cancellationToken)
    {
        // Get planning details + activities
        var result = await (
            from pd in _db.Set<PlanningDetail>()
            join activity in _db.Set<Activity>()
                on pd.ActivityID equals activity.ID
            where pd.AreaID == request.AreaID
                  && pd.IsActive
                  && pd.Planning != null
                  && pd.Planning.IsActive
            select new
            {
                pd.ActivityID,
                ActivityName = activity.ActivityName,
                pd.TargetQuantity,
                pd.UOMID
            }
        ).ToListAsync(cancellationToken);

        if (!result.Any())
        {
            return new List<ActivityWiseQuantityBySectionModel>();
        }

        // Get distinct UOM IDs
        var uomIds = result
            .Where(x => x.UOMID > 0)
            .Select(x => x.UOMID)
            .Distinct()
            .ToArray();

        var uomMap = new Dictionary<int, (string Name, string ShortName)>();

        if (uomIds.Length > 0)
        {
            var dbContext = _db as DbContext;

            if (dbContext == null)
            {
                throw new InvalidOperationException(
                    "IExecutionDbContext is not a DbContext.");
            }

            var connection = dbContext.Database.GetDbConnection();

            if (connection.State != System.Data.ConnectionState.Open)
            {
                await connection.OpenAsync(cancellationToken);
            }

            try
            {
                using var command = connection.CreateCommand();

                command.CommandText = @"
                SELECT 
                    ""ID"",
                    ""UOMName"",
                    ""UOMShortName""
                FROM public.""UnitOfMeasurement""
                WHERE ""ID"" = ANY(@uomIds)";

                var parameter = command.CreateParameter();
                parameter.ParameterName = "@uomIds";
                parameter.Value = uomIds;

                command.Parameters.Add(parameter);

                using var reader = await command.ExecuteReaderAsync(cancellationToken);

                while (await reader.ReadAsync(cancellationToken))
                {
                    var id = reader.GetInt32(0);

                    var name = reader.IsDBNull(1)
                        ? string.Empty
                        : reader.GetString(1);

                    var shortName = reader.IsDBNull(2)
                        ? string.Empty
                        : reader.GetString(2);

                    uomMap[id] = (name, shortName);
                }
            }
            finally
            {
                await connection.CloseAsync();
            }
        }

        // Map final response
        return result.Select(x =>
        {
            uomMap.TryGetValue(x.UOMID, out var uom);

            return new ActivityWiseQuantityBySectionModel
            {
                ActivityID = x.ActivityID,
                ActivityName = x.ActivityName,
                TargetQuantity = x.TargetQuantity,
                UOMID = x.UOMID,
                UOMName = uom.Name,
                UOMShortName = uom.ShortName
            };
        }).ToList();
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

            // No details required for duplicate-date validation
            Array.Empty<SiteDailyProgressDetailModel>(),

            // No hindrances required
            Array.Empty<SiteDailyProgressHindranceModel>(),

            // No photos required
            Array.Empty<SiteDailyProgressPhotoModel>()
        );
    }
}
