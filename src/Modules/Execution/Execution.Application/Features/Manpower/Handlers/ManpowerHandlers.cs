
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
using ManpowerEntity = Himapp.Execution.Domain.Entities.Manpower;


namespace Himapp.Execution.Application.Features.Manpower.Handlers;

internal sealed class ManpowerHandlers :
    IRequestHandler<GetAllManpowersQuery, IReadOnlyCollection<ManpowerModel>>,
    IRequestHandler<GetManpowerByIdQuery, ManpowerModel?>,
    IRequestHandler<CreateManpowerCommand, ManpowerModel>,
    IRequestHandler<UpdateManpowerCommand, ManpowerModel?>,
    IRequestHandler<DeleteManpowerCommand, bool>,
    IRequestHandler<DeleteManpowerActionCommand, bool>,
    IRequestHandler<GetManpowerByProjectID, DataSet>,
    IRequestHandler<GetLastManpowerBySectionIDQuery, ManpowerModel?>
{
    private readonly IExecutionDbContext _db;
    private readonly ICurrentUser _currentUser;
    public ManpowerHandlers(IExecutionDbContext db, ICurrentUser currentUser) => (_db, _currentUser) = (db, currentUser);

    private int CurrentUserId => _currentUser.UserId ?? throw new UnauthorizedAccessException("An authenticated user is required.");

    public async Task<IReadOnlyCollection<ManpowerModel>> Handle(GetAllManpowersQuery request, CancellationToken cancellationToken)
    {
        return await _db.Set<ManpowerEntity>()
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
                    string.Empty,
                    d.ActivityID,
                    string.Empty,
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
        var m = await _db.Set<ManpowerEntity>()
            .AsNoTracking()
            .Include(x => x.ManpowerDetail)
            .FirstOrDefaultAsync(x => x.ID == request.Id && x.IsActive, cancellationToken);
        if (m is null) return null;

        var details = m.ManpowerDetail?.Select(d => new ManpowerDetailModel(
            d.ID,
                    d.UniqueID,
                    d.ContractorID,
                    string.Empty,
                    d.ActivityID,
                    string.Empty,
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

        var entity = new ManpowerEntity
        {
            UniqueID = Guid.NewGuid(),
            ProjectID = r.ProjectId,
            SectionID = r.SectionId,
            EntryDate = r.EntryDate,
            Remarks = r.Remarks,
            StateID = 3,
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
                var detail = new ManpowerDetail
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
                    CreatedDate = DateTime.UtcNow,
                    LastModifiedBy = r.LastModifiedBy,
                    LastModifiedDate = DateTime.UtcNow,
                    Manpower = entity
                };

                entity.ManpowerDetail?.Add(detail);
            }
        }

        _db.Set<ManpowerEntity>().Add(entity);
        await _db.SaveChangesAsync(cancellationToken);

        var details = entity.ManpowerDetail?.Select(d => new ManpowerDetailModel(d.ID, d.UniqueID, d.ContractorID, string.Empty, d.ActivityID, string.Empty, d.SkilledCount, d.UnskilledCount, d.OtherCount, d.IsDepartment, d.TotalCount)).ToArray() ?? Array.Empty<ManpowerDetailModel>();

        return new ManpowerModel(entity.ID, entity.UniqueID, entity.ProjectID, entity.SectionID, entity.EntryDate, entity.Remarks, entity.StateID, entity.IsActive, entity.CreatedBy, entity.CreatedDate, entity.LastModifiedBy, entity.LastModifiedDate, details);
    }

    public async Task<ManpowerModel?> Handle(UpdateManpowerCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.Set<ManpowerEntity>().Include(x => x.ManpowerDetail).FirstOrDefaultAsync(x => x.ID == request.Id && x.IsActive, cancellationToken);
        if (entity is null) return null;
        var r = request.Request;

        entity.SectionID = r.SectionId;
        entity.EntryDate = r.EntryDate;
        entity.Remarks = r.Remarks;
        entity.StateID = r.StateId;
        entity.IsActive = r.IsActive;
        entity.LastModifiedBy = r.LastModifiedBy;
        entity.LastModifiedDate = DateTime.UtcNow;

        if (entity.ManpowerDetail != null && entity.ManpowerDetail.Any())
        {
            _db.Set<ManpowerDetail>().RemoveRange(entity.ManpowerDetail);
            entity.ManpowerDetail.Clear();
        }

        if (r.Details?.Any() == true)
        {
            foreach (var d in r.Details)
            {
                var detail = new ManpowerDetail
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
                    CreatedDate = DateTime.UtcNow,
                    LastModifiedBy = r.LastModifiedBy,
                    LastModifiedDate = DateTime.UtcNow,
                    Manpower = entity
                };

                entity.ManpowerDetail?.Add(detail);
            }
        }

        await _db.SaveChangesAsync(cancellationToken);

        var details = entity.ManpowerDetail?.Select(d => new ManpowerDetailModel(d.ID, d.UniqueID, d.ContractorID, string.Empty, d.ActivityID, string.Empty, d.SkilledCount, d.UnskilledCount, d.OtherCount, d.IsDepartment, d.TotalCount)).ToArray() ?? Array.Empty<ManpowerDetailModel>();

        return new ManpowerModel(entity.ID, entity.UniqueID, entity.ProjectID, entity.SectionID, entity.EntryDate, entity.Remarks, entity.StateID, entity.IsActive, entity.CreatedBy, entity.CreatedDate, entity.LastModifiedBy, entity.LastModifiedDate, details);
    }

    public async Task<bool> Handle(DeleteManpowerCommand request, CancellationToken cancellationToken)
    {
        var userId = CurrentUserId;
        var entity = await _db.Set<ManpowerEntity>().Include(d => d.ManpowerDetail).FirstOrDefaultAsync(x => x.ID == request.Id && x.IsActive, cancellationToken);
        if (entity is null) return false;

        entity.IsActive = false;
        entity.LastModifiedBy = userId;
        entity.LastModifiedDate = DateTime.UtcNow;

        if (entity.ManpowerDetail != null)
        {
            foreach (var dd in entity.ManpowerDetail)
            {
                dd.IsActive = false;
                dd.LastModifiedBy = userId;
                dd.LastModifiedDate = DateTime.UtcNow;
            }
        }

        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> Handle(DeleteManpowerActionCommand request, CancellationToken cancellationToken)
    {
        var model = request.addTransactionActionHistoryDTO;
        var entity = await _db.Set<ManpowerEntity>().Include(d => d.ManpowerDetail).FirstOrDefaultAsync(x => x.ID == model.ProgramRowId, cancellationToken);
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

    public async Task<DataSet> Handle(GetManpowerByProjectID request, CancellationToken cancellationToken)
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
        using (var cmd = new NpgsqlCommand("SELECT * FROM execution.uspgetmanpowerbyprojectid(@p_projectid,@p_filtercolumn,@p_filtervalue,@p_pageindex,@p_pagesize,@p_sortcolumn,@p_isactive)", conn))
        {
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = 10;
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
        using (var cmd2 = new NpgsqlCommand("SELECT cnt FROM execution.uspgetmanpowercountbyprojectid(@p_projectid,@p_filtercolumn,@p_filtervalue,@p_pageindex,@p_pagesize,@p_sortcolumn,@p_isactive)", conn))
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

    public async Task<ManpowerModel?> Handle(GetLastManpowerBySectionIDQuery request, CancellationToken cancellationToken)
    {
        var manpower = await _db.Set<Domain.Entities.Manpower>().AsNoTracking().Include(x => x.ManpowerDetail).Where(x => x.ProjectID == request.ProjectId && x.SectionID == request.SectionId && x.IsActive)
            .OrderByDescending(x => x.EntryDate)
            .ThenByDescending(x => x.ID)
            .FirstOrDefaultAsync(cancellationToken);

        if (manpower == null)
            return null;

        var detailsData = manpower.ManpowerDetail?
            .Where(x => x.IsActive)
            .ToList()
            ?? new List<Domain.Entities.ManpowerDetail>();


        // =========================================================
        // CONTRACTOR IDs
        // =========================================================

        var contractorIds = detailsData
            .Where(x => x.ContractorID > 0)
            .Select(x => x.ContractorID)
            .Distinct()
            .ToArray();


        // =========================================================
        // ACTIVITY IDs
        // =========================================================

        var activityIds = detailsData
            .Where(x => x.ActivityID > 0)
            .Select(x => x.ActivityID)
            .Distinct()
            .ToArray();


        var contractorMap = new Dictionary<int, string>();
        var activityMap = new Dictionary<int, string>();


        // =========================================================
        // FETCH CONTRACTOR + ACTIVITY NAMES
        // =========================================================

        if (contractorIds.Length > 0 || activityIds.Length > 0)
        {
            await using var connection = _db.Database.GetDbConnection();

            if (connection.State != System.Data.ConnectionState.Open)
                await connection.OpenAsync(cancellationToken);


            // -----------------------------------------------------
            // CONTRACTORS
            // -----------------------------------------------------

            if (contractorIds.Length > 0)
            {
                using var command = connection.CreateCommand();

                command.CommandText = @"
                SELECT ""ID"", ""PartyName""
                FROM public.""PartyMaster""
                WHERE ""ID"" = ANY(@contractorIds)";

                var parameter = command.CreateParameter();
                parameter.ParameterName = "@contractorIds";
                parameter.Value = contractorIds;

                command.Parameters.Add(parameter);

                using var reader =
                    await command.ExecuteReaderAsync(cancellationToken);

                while (await reader.ReadAsync(cancellationToken))
                {
                    var id = reader.GetInt32(0);

                    var name = reader.IsDBNull(1)
                        ? string.Empty
                        : reader.GetString(1);

                    contractorMap[id] = name;
                }
            }


            // -----------------------------------------------------
            // ACTIVITIES
            // -----------------------------------------------------

            if (activityIds.Length > 0)
            {
                using var command = connection.CreateCommand();

                command.CommandText = @"
                SELECT ""ID"", ""ActivityName""
                FROM execution.""Activities""
                WHERE ""ID"" = ANY(@activityIds)";

                var parameter = command.CreateParameter();
                parameter.ParameterName = "@activityIds";
                parameter.Value = activityIds;

                command.Parameters.Add(parameter);

                using var reader =
                    await command.ExecuteReaderAsync(cancellationToken);

                while (await reader.ReadAsync(cancellationToken))
                {
                    var id = reader.GetInt32(0);

                    var name = reader.IsDBNull(1)
                        ? string.Empty
                        : reader.GetString(1);

                    activityMap[id] = name;
                }
            }
        }


        // =========================================================
        // BUILD DETAILS
        // =========================================================

        var details = detailsData
            .Select(d => new ManpowerDetailModel(
                d.ID,
                d.UniqueID,

                d.ContractorID,
                contractorMap.TryGetValue(
                    d.ContractorID,
                    out var contractorName)
                    ? contractorName
                    : string.Empty,

                d.ActivityID,
                activityMap.TryGetValue(
                    d.ActivityID,
                    out var activityName)
                    ? activityName
                    : string.Empty,

                d.SkilledCount,
                d.UnskilledCount,
                d.OtherCount,
                d.IsDepartment,
                d.TotalCount
            ))
            .ToList();


        // =========================================================
        // RETURN
        // =========================================================

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

