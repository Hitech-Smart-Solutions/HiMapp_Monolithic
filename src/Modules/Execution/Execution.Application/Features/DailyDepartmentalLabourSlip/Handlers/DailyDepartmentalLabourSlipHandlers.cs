using Himapp.Execution.Application.Features.DailyDepartmentalLabourSlip.Commands;
using Himapp.Execution.Application.Features.DailyDepartmentalLabourSlip.Models;
using Himapp.Execution.Application.Features.DailyDepartmentalLabourSlip.Queries;
using Himapp.Execution.Application.Features.Manpower.Queries;
using Himapp.Execution.Application.Features.SiteDailyProgress.Queries;
using Himapp.Execution.Contracts;
using Himapp.Execution.Contracts.References;
using Himapp.Execution.Domain.Entities;
using Himapp.SharedKernel.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using NpgsqlTypes;
using System.Data;
using DDLSEntity = Himapp.Execution.Domain.Entities.DailyDepartmentalLabourSlip;

namespace Himapp.Execution.Application.Features.DailyDepartmentalLabourSlip.Handlers;

internal sealed class DailyDepartmentalLabourSlipHandlers :
    IRequestHandler<GetAllDailyDepartmentalLabourSlipsQuery, IEnumerable<DailyDepartmentalLabourSlipModel>>,
    IRequestHandler<GetDailyDepartmentalLabourSlipByIdQuery, DailyDepartmentalLabourSlipModel?>,
    IRequestHandler<CreateDailyDepartmentalLabourSlipCommand, DailyDepartmentalLabourSlipModel>,
    IRequestHandler<UpdateDailyDepartmentalLabourSlipCommand, DailyDepartmentalLabourSlipModel?>,
    IRequestHandler<DeleteDailyDepartmentalLabourSlipCommand, bool>,
    IRequestHandler<DeleteDDLSCommand, bool>,
    IRequestHandler<GetDailyDepartmentalLabourSlipsByProjectID, DataSet>,
    IRequestHandler<GetDailyDepartmentalLabourSlipByIdAndProgramId, GetDailyDepartmentalLabourSlipByIdModel?>
{
    private readonly IExecutionDbContext _db;
    private readonly IDdlSlipCodeGenerator _codeGenerator;
    private readonly ILogger<DailyDepartmentalLabourSlipHandlers> _logger;
    private readonly ICurrentUser _currentUser;

    public DailyDepartmentalLabourSlipHandlers(IExecutionDbContext db, ICurrentUser currentUser, IDdlSlipCodeGenerator codeGenerator, ILogger<DailyDepartmentalLabourSlipHandlers>? logger = null) => (_db, _currentUser, _codeGenerator, _logger) = (db, currentUser, codeGenerator, logger ?? Microsoft.Extensions.Logging.Abstractions.NullLogger<DailyDepartmentalLabourSlipHandlers>.Instance);
    private int CurrentUserId => _currentUser.UserId ?? 0;
    public async Task<IEnumerable<DailyDepartmentalLabourSlipModel>> Handle(GetAllDailyDepartmentalLabourSlipsQuery request, CancellationToken cancellationToken)
    {
        return await _db.Set<DDLSEntity>()
            .AsNoTracking()
            .Where(d => d.IsActive)
            .Select(d => new DailyDepartmentalLabourSlipModel(
                d.ID,
                d.UniqueID,
                d.ProjectID,
                d.SlipDate,
                d.DDLSlipCode,
                d.IssueNumber,
                d.PartyID,
                d.Remarks,
                d.StatusID,
                d.IsActive,
                d.CreatedBy,
                 d.CreatedDate,
                d.LastModifiedBy,
                d.LastModifiedDate,
                Array.Empty<DailyDepartmentalLabourSlipDetailsModel>()))
            .ToArrayAsync(cancellationToken);
    }

    public async Task<bool> Handle(DeleteDDLSCommand request, CancellationToken cancellationToken)
    {
        var model = request.addTransactionActionHistoryDTO;
        var entity = await _db.Set<DDLSEntity>().FirstOrDefaultAsync(a => a.ID == model.ProgramRowId, cancellationToken);

        if (entity is null) return false;

        // Mark child detail records active/inactive
        var details = await _db.Set<DailyDepartmentalLabourSlipDetails>()
            .Where(x => x.DDLSlipID == model.ProgramRowId)
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

    public async Task<DailyDepartmentalLabourSlipModel?> Handle(GetDailyDepartmentalLabourSlipByIdQuery request, CancellationToken cancellationToken)
    {
        var d = await _db.Set<DDLSEntity>()
            .AsNoTracking()
            .Include(x => x.DailyDepartmentalLabourSlipDetails)
            .FirstOrDefaultAsync(x => x.ID == request.Id && x.IsActive, cancellationToken);
        if (d is null) return null;

        var details = d.DailyDepartmentalLabourSlipDetails?.Select(dd => new DailyDepartmentalLabourSlipDetailsModel(
            dd.ID,
            dd.UniqueID,
            dd.LabourCategoryTypeID,
            dd.IsLumSumWork,
            dd.NumOfLabour,
            dd.FromTime,
            dd.TOTime,
            dd.LunchHour,
            dd.WorkingHours,
            dd.WorkLocationID,
            dd.ActivityID,
            dd.ActivityDetails,
            dd.UOMID,
            dd.Quantity,
            dd.DebitPartyID,
            dd.Remarks)).ToArray() ?? Array.Empty<DailyDepartmentalLabourSlipDetailsModel>();

        return new DailyDepartmentalLabourSlipModel(d.ID, d.UniqueID, d.ProjectID, d.SlipDate, d.DDLSlipCode, d.IssueNumber, d.PartyID, d.Remarks, d.StatusID, d.IsActive, d.CreatedBy, d.CreatedDate, d.LastModifiedBy, d.LastModifiedDate, details);
    }

    public async Task<DailyDepartmentalLabourSlipModel> Handle(CreateDailyDepartmentalLabourSlipCommand request, CancellationToken cancellationToken)
    {
        var r = request.Request;
        var userId = CurrentUserId;
        // Generate DDLSlipCode project-wise before constructing the entity so we can log and inspect it
        var generatedCode = await _codeGenerator.GenerateDDLSlipCodeAsync(r.ProjectId, cancellationToken);
        _logger.LogDebug("DDL slip code generated for ProjectId {ProjectId}: '{Code}'", r.ProjectId, generatedCode);

        var entity = new DDLSEntity
        {
            UniqueID = Guid.NewGuid(),
            ProjectID = r.ProjectId,
            DDLSlipCode = string.IsNullOrWhiteSpace(generatedCode) ? null : generatedCode,
            IssueNumber = r.IssueNumber,
            PartyID = r.PartyID,
            SlipDate = r.SlipDate.HasValue ? DateTime.SpecifyKind(r.SlipDate.Value.Date, DateTimeKind.Utc) : DateTime.UtcNow.Date,
            Remarks = r.Remarks,
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
                var workingHours = CalculateWorkingHours(d.FromTime, d.ToTime, d.LunchHour);
                var detail = new DailyDepartmentalLabourSlipDetails
                {
                    UniqueID = Guid.NewGuid(),
                    DDLSlipID = entity.ID,
                    LabourCategoryTypeID = d.LabourCategoryTypeId,
                    IsLumSumWork = d.IsLumSumWork,
                    NumOfLabour = d.NumOfLabour,
                    FromTime = d.FromTime,
                    TOTime = d.ToTime,
                    LunchHour = d.LunchHour,
                    WorkingHours = workingHours,
                    WorkLocationID = d.WorkLocationId,
                    ActivityID = d.ActivityID,
                    ActivityDetails = d.ActivityDetails,
                    UOMID = d.UomId,
                    Quantity = d.Quantity,
                    DebitPartyID = d.DebitPartyId,
                    Remarks = d.Remarks,
                    IsActive = true,
                    CreatedBy = userId,
                    CreatedDate = DateTime.UtcNow,
                    LastModifiedBy = userId,
                    LastModifiedDate = DateTime.UtcNow
                };

                entity.DailyDepartmentalLabourSlipDetails?.Add(detail);
            }
        }

        _db.Set<DDLSEntity>().Add(entity);
        await _db.SaveChangesAsync(cancellationToken);

        var details = entity.DailyDepartmentalLabourSlipDetails?.Select(dd => new DailyDepartmentalLabourSlipDetailsModel(
            dd.ID,
            dd.UniqueID,
            dd.LabourCategoryTypeID,
            dd.IsLumSumWork,
            dd.NumOfLabour,
            dd.FromTime,
            dd.TOTime,
            dd.LunchHour,
            dd.WorkingHours,
            dd.WorkLocationID,
            dd.ActivityID,
            dd.ActivityDetails,
            dd.UOMID,
            dd.Quantity,
            dd.DebitPartyID,
            dd.Remarks)).ToArray() ?? Array.Empty<DailyDepartmentalLabourSlipDetailsModel>();

        return new DailyDepartmentalLabourSlipModel(entity.ID, entity.UniqueID, entity.ProjectID, entity.SlipDate, entity.DDLSlipCode, entity.IssueNumber, entity.PartyID, entity.Remarks, entity.StatusID, entity.IsActive, entity.CreatedBy, entity.CreatedDate, entity.LastModifiedBy, entity.LastModifiedDate, details);
    }

    public async Task<DailyDepartmentalLabourSlipModel?> Handle(UpdateDailyDepartmentalLabourSlipCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.Set<DDLSEntity>()
            .Include(d => d.DailyDepartmentalLabourSlipDetails)
            .FirstOrDefaultAsync(x => x.ID == request.Id && x.IsActive, cancellationToken);
        if (entity is null) return null;

        var r = request.Request;

        entity.ProjectID = r.ProjectId;
        entity.IssueNumber = entity.IssueNumber;
        entity.SlipDate = r.SlipDate.HasValue ? DateTime.SpecifyKind(r.SlipDate.Value.Date, DateTimeKind.Utc) : entity.SlipDate;
        entity.PartyID = r.PartyID;
        entity.Remarks = r.Remarks ?? entity.Remarks;
        entity.StatusID = r.StatusID;
        entity.LastModifiedBy = r.LastModifiedBy;
        entity.LastModifiedDate = DateTime.UtcNow;

        // Remove existing details and add new ones
        if (entity.DailyDepartmentalLabourSlipDetails != null && entity.DailyDepartmentalLabourSlipDetails.Any())
        {
            _db.Set<DailyDepartmentalLabourSlipDetails>().RemoveRange(entity.DailyDepartmentalLabourSlipDetails);
            entity.DailyDepartmentalLabourSlipDetails.Clear();
        }

        if (r.Details?.Any() == true)
        {
            foreach (var d in r.Details)
            {
                var workingHours = CalculateWorkingHours(d.FromTime, d.ToTime, d.LunchHour);
                var detail = new DailyDepartmentalLabourSlipDetails
                {
                    UniqueID = Guid.NewGuid(),
                    LabourCategoryTypeID = d.LabourCategoryTypeId,
                    IsLumSumWork = d.IsLumSumWork,
                    NumOfLabour = d.NumOfLabour,
                    FromTime = d.FromTime,
                    TOTime = d.ToTime,
                    LunchHour = d.LunchHour,
                    WorkingHours = workingHours,
                    WorkLocationID = d.WorkLocationId,
                    ActivityID = d.ActivityID,
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
            dd.IsLumSumWork,
            dd.NumOfLabour,
            dd.FromTime,
            dd.TOTime,
            dd.LunchHour,
            dd.WorkingHours,
            dd.WorkLocationID,
            dd.ActivityID,
            dd.ActivityDetails,
            dd.UOMID,
            dd.Quantity,
            dd.DebitPartyID,
            dd.Remarks)).ToArray() ?? Array.Empty<DailyDepartmentalLabourSlipDetailsModel>();

        return new DailyDepartmentalLabourSlipModel(entity.ID, entity.UniqueID, entity.ProjectID, entity.SlipDate, entity.DDLSlipCode, entity.IssueNumber, entity.PartyID, entity.Remarks, entity.StatusID, entity.IsActive, entity.CreatedBy, entity.CreatedDate, entity.LastModifiedBy, entity.LastModifiedDate, details);
    }

    public async Task<bool> Handle(DeleteDailyDepartmentalLabourSlipCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.Set<DDLSEntity>()
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

    public async Task<DataSet> Handle(GetDailyDepartmentalLabourSlipsByProjectID request, CancellationToken cancellationToken)
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
        using (var cmd = new NpgsqlCommand("SELECT * FROM execution.uspgetdailydepartmentallabourslipbyprojectid(@p_projectid,@p_filtercolumn,@p_filtervalue,@p_pageindex,@p_pagesize,@p_sortcolumn,@p_isactive)", conn))
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
        using (var cmd2 = new NpgsqlCommand("SELECT cnt FROM execution.uspgetdailydepartmentallabourslipcountbyprojectid(@p_projectid,@p_filtercolumn,@p_filtervalue,@p_pageindex,@p_pagesize,@p_sortcolumn,@p_isactive)", conn))
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

    private static decimal CalculateWorkingHours(DateTime fromTime, DateTime toTime, decimal? lunchHour)
    {
        if (toTime <= fromTime)
            return 0;

        var totalMinutes = (decimal)(toTime - fromTime).TotalMinutes;

        var lunchMinutes = (lunchHour ?? 0) * 60;

        var workingMinutes = Math.Max(
            totalMinutes - lunchMinutes,
            0
        );

        return Math.Round(workingMinutes / 60m, 2);
    }

    public async Task<GetDailyDepartmentalLabourSlipByIdModel?> Handle(GetDailyDepartmentalLabourSlipByIdAndProgramId request, CancellationToken cancellationToken)
    {
        var connection = _db.Database.GetDbConnection();

        const string sql = """
            SELECT
                ddl."ID",
                ddl."UniqueID",
                
                ddl."ProjectID",
                pm."ProjectName",
                ddl."SlipDate",
                ddl."DDLSlipCode",
                ddl."IssueNumber",
                
                ddl."PartyID",
                dvm."NAME" AS "ContractorName",
                ddl."Remarks",
                ddl."StatusID",
                ddl."IsActive",
               
                ddl."CreatedBy",
                um."UserName" AS "CreatedName",
               
                ddl."CreatedDate",
                ddl."LastModifiedBy",
                ddl."LastModifiedDate",

                approval."NextActionOn" AS "IsAwaitingApprovalForId"

            FROM execution."DailyDepartmentalLabourSlips" ddl

            LEFT JOIN public."ProjectMaster" pm
                ON pm."ID" = ddl."ProjectID"

            LEFT JOIN public."DynamicsVendorMaster" dvm
                ON dvm."ID" = ddl."PartyID"

            LEFT JOIN public."UserMaster" um
                ON um."ID" = ddl."CreatedBy"

            LEFT JOIN public."ApprovalActions" approval
                ON approval."ProgramRowID" = ddl."ID"
                AND approval."ProgramID" = @ProgramId
                AND approval."StatusID" = 2

            WHERE
                ddl."ID" = @Id
                AND ddl."IsActive" = TRUE;


            SELECT
                detail."ID",
                detail."UniqueID",

                detail."LabourCategoryTypeID",
                acd."Name",

                detail."IsLumSumWork",
                detail."NumOfLabour",
                detail."FromTime",
                detail."TOTime",
                detail."LunchHour",
                detail."WorkingHours",
                detail."WorkLocationID",

                detail."ActivityID",
                act."ActivityName",

                detail."ActivityDetails",

                detail."UOMID",
                uomDetails."UOMShortName",
                
                detail."Quantity",
                
                detail."DebitPartyID",
                debitVendor."NAME" AS "DebitPartyName",
                
                detail."Remarks"

            FROM execution."DailyDepartmentalLabourSlipDetails" detail
            
            LEFT JOIN public."DynamicsVendorMaster" debitVendor
                ON debitVendor."ID" = detail."DebitPartyID"
            
            LEFT JOIN public."UnitOfMeasurement" uomDetails
                ON uomDetails."ID" = detail."UOMID"

            LEFT JOIN execution."Activities" act
                ON act."ID" = detail."ActivityID"

            LEFT join execution."ActivityCategoryDetails" acd
                ON acd."ID" = detail."LabourCategoryTypeID"
            
            WHERE detail."DDLSlipID" = @Id

            ORDER BY detail."ID";
            """;

        try
        {
            if (connection.State != ConnectionState.Open)
            {
                await connection.OpenAsync(cancellationToken);
            }

            using var command = connection.CreateCommand();

            command.CommandText = sql;

            // ID parameter
            var idParameter = command.CreateParameter();
            idParameter.ParameterName = "@Id";
            idParameter.Value = request.Id;
            command.Parameters.Add(idParameter);

            // ProgramID parameter
            var programIdParameter = command.CreateParameter();
            programIdParameter.ParameterName = "@ProgramId";
            programIdParameter.Value = request.ProgramId;
            command.Parameters.Add(programIdParameter);

            using var reader =
                await command.ExecuteReaderAsync(cancellationToken);

            // =========================================================
            // HEADER
            // =========================================================

            if (!await reader.ReadAsync(cancellationToken))
            {
                return null;
            }

            var idOrdinal =
                reader.GetOrdinal("ID");

            var uniqueIdOrdinal =
                reader.GetOrdinal("UniqueID");

            var projectIdOrdinal =
                reader.GetOrdinal("ProjectID");

            var projectNameOrdinal = reader.GetOrdinal("ProjectName");

            var slipDateOrdinal =
                reader.GetOrdinal("SlipDate");

            var ddlSlipCodeOrdinal =
                reader.GetOrdinal("DDLSlipCode");

            var issueNumberOrdinal =
                reader.GetOrdinal("IssueNumber");

            var partyIdOrdinal =
                reader.GetOrdinal("PartyID");

            var contractorNameOrdinal = reader.GetOrdinal("ContractorName");

            var remarksOrdinal =
                reader.GetOrdinal("Remarks");

            var statusIdOrdinal =
                reader.GetOrdinal("StatusID");

            var isActiveOrdinal =
                reader.GetOrdinal("IsActive");

            var createdByOrdinal =
                reader.GetOrdinal("CreatedBy");

            var createdNameOrdinal = reader.GetOrdinal("CreatedName");

            var createdDateOrdinal =
                reader.GetOrdinal("CreatedDate");

            var lastModifiedByOrdinal =
                reader.GetOrdinal("LastModifiedBy");

            var lastModifiedDateOrdinal =
                reader.GetOrdinal("LastModifiedDate");

            var awaitingApprovalOrdinal =
                reader.GetOrdinal("IsAwaitingApprovalForId");

            var id = reader.GetInt32(idOrdinal);

            var uniqueId = reader.GetGuid(uniqueIdOrdinal);

            int? projectId =
                reader.IsDBNull(projectIdOrdinal)
                    ? null
                    : reader.GetInt32(projectIdOrdinal);

            DateTime? slipDate =
                reader.IsDBNull(slipDateOrdinal)
                    ? null
                    : reader.GetDateTime(slipDateOrdinal);

            string? ddlSlipCode =
                reader.IsDBNull(ddlSlipCodeOrdinal)
                    ? null
                    : reader.GetString(ddlSlipCodeOrdinal);

            string? issueNumber =
                reader.IsDBNull(issueNumberOrdinal)
                    ? null
                    : reader.GetString(issueNumberOrdinal);

            int? partyId =
                reader.IsDBNull(partyIdOrdinal)
                    ? null
                    : reader.GetInt32(partyIdOrdinal);

            string? projectName =
                reader.IsDBNull(projectNameOrdinal)
                    ? null
                    : reader.GetString(projectNameOrdinal);

            string? contractorName =
                reader.IsDBNull(contractorNameOrdinal)
                    ? null
                    : reader.GetString(contractorNameOrdinal);

            string? remarks =
                reader.IsDBNull(remarksOrdinal)
                    ? null
                    : reader.GetString(remarksOrdinal);


            string? createdName =
                reader.IsDBNull(createdNameOrdinal)
                    ? null
                    : reader.GetString(createdNameOrdinal);

            var statusId =
                reader.GetInt32(statusIdOrdinal);

            var isActive =
                reader.GetBoolean(isActiveOrdinal);

            var createdBy =
                reader.GetInt32(createdByOrdinal);

            var createdDate =
                reader.GetDateTime(createdDateOrdinal);

            var lastModifiedBy =
                reader.GetInt32(lastModifiedByOrdinal);

            var lastModifiedDate =
                reader.GetDateTime(lastModifiedDateOrdinal);

            int? isAwaitingApprovalForId =
                reader.IsDBNull(awaitingApprovalOrdinal)
                    ? null
                    : reader.GetInt32(awaitingApprovalOrdinal);

            // =========================================================
            // DETAILS
            // =========================================================

            var details =
                new List<GetDailyDepartmentalLabourSlipDetailsModel>();

            if (await reader.NextResultAsync(cancellationToken))
            {
                var detailIdOrdinal =
                    reader.GetOrdinal("ID");

                var detailUniqueIdOrdinal =
                    reader.GetOrdinal("UniqueID");

                var labourCategoryTypeIdOrdinal =
                    reader.GetOrdinal("LabourCategoryTypeID");

                var labourCategoryTypeNameOrdinal = reader.GetOrdinal("Name");

                var isLumSumWorkOrdinal =
                    reader.GetOrdinal("IsLumSumWork");

                var numOfLabourOrdinal =
                    reader.GetOrdinal("NumOfLabour");

                var fromTimeOrdinal =
                    reader.GetOrdinal("FromTime");

                var toTimeOrdinal =
                    reader.GetOrdinal("TOTime");

                var lunchHourOrdinal =
                    reader.GetOrdinal("LunchHour");

                var workingHoursOrdinal =
                    reader.GetOrdinal("WorkingHours");

                var workLocationIdOrdinal =
                    reader.GetOrdinal("WorkLocationID");

                var activityIdOrdinal =
                    reader.GetOrdinal("ActivityID");

                var activityNameOrdinal =
                    reader.GetOrdinal("ActivityName");

                var activityDetailsOrdinal =
                    reader.GetOrdinal("ActivityDetails");

                var uomIdOrdinal =
                    reader.GetOrdinal("UOMID");


                var uomShortNameOrdinal =
                    reader.GetOrdinal("UOMShortName");

                var quantityOrdinal =
                    reader.GetOrdinal("Quantity");

                var debitPartyIdOrdinal =
                    reader.GetOrdinal("DebitPartyID");

                var debitPartyNameOrdinal =
                    reader.GetOrdinal("DebitPartyName");

                var detailRemarksOrdinal =
                    reader.GetOrdinal("Remarks");

                while (await reader.ReadAsync(cancellationToken))
                {
                    details.Add(
                        new GetDailyDepartmentalLabourSlipDetailsModel(
                            reader.GetInt32(detailIdOrdinal),

                            reader.GetGuid(detailUniqueIdOrdinal),

                            reader.GetInt32(labourCategoryTypeIdOrdinal),

                             reader.IsDBNull(labourCategoryTypeNameOrdinal)
                                ? null
                                : reader.GetString(labourCategoryTypeNameOrdinal),

                            reader.GetBoolean(isLumSumWorkOrdinal),

                            reader.GetInt32(numOfLabourOrdinal),

                            reader.IsDBNull(fromTimeOrdinal)
                                ? null
                                : reader.GetDateTime(fromTimeOrdinal),

                            reader.IsDBNull(toTimeOrdinal)
                                ? null
                                : reader.GetDateTime(toTimeOrdinal),

                            reader.IsDBNull(lunchHourOrdinal)
                                ? null
                                : reader.GetDecimal(lunchHourOrdinal),

                            reader.IsDBNull(workingHoursOrdinal)
                                ? null
                                : reader.GetDecimal(workingHoursOrdinal),

                            reader.IsDBNull(workLocationIdOrdinal)
                                ? null
                                : reader.GetInt32(workLocationIdOrdinal),

                            reader.IsDBNull(activityIdOrdinal)
                                ? null
                                : reader.GetInt32(activityIdOrdinal),

                            reader.IsDBNull(activityNameOrdinal)
                                ? null
                                : reader.GetString(activityNameOrdinal),

                            reader.IsDBNull(activityDetailsOrdinal)
                                ? null
                                : reader.GetString(activityDetailsOrdinal),

                            reader.IsDBNull(uomIdOrdinal)
                                ? null
                                : reader.GetInt32(uomIdOrdinal),

                            reader.IsDBNull(uomShortNameOrdinal)
                                ? null
                                : reader.GetString(uomShortNameOrdinal),

                            reader.IsDBNull(quantityOrdinal)
                                ? null
                                : reader.GetDecimal(quantityOrdinal),

                            reader.IsDBNull(debitPartyIdOrdinal)
                                ? null
                                : reader.GetInt32(debitPartyIdOrdinal),

                            reader.IsDBNull(debitPartyNameOrdinal)
                                ? null
                                : reader.GetString(debitPartyNameOrdinal),

                            reader.IsDBNull(detailRemarksOrdinal)
                                ? null
                                : reader.GetString(detailRemarksOrdinal)
                        )
                    );
                }
            }

            return new GetDailyDepartmentalLabourSlipByIdModel(id, uniqueId, projectId, projectName, slipDate, ddlSlipCode, issueNumber, partyId, contractorName, remarks, statusId, isActive, createdBy, createdName,
                    createdDate,
                    lastModifiedBy,
                    lastModifiedDate,
                    details,
                    isAwaitingApprovalForId);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error getting DDLS by Id {Id} and ProgramId {ProgramId}",
                request.Id,
                request.ProgramId);

            throw;
        }
    }
}
