using Himapp.Execution.Application.Features.DailyDepartmentalLabourSlip.Commands;
using Himapp.Execution.Application.Features.DailyDepartmentalLabourSlip.Models;
using Himapp.Execution.Application.Features.DailyDepartmentalLabourSlip.Queries;
using Himapp.Execution.Application.Features.Manpower.Queries;
using Himapp.Execution.Application.Features.SiteDailyProgress.Queries;
using Himapp.Execution.Contracts;
using Himapp.Execution.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using NpgsqlTypes;
using System.Data;

namespace Himapp.Execution.Application.Features.DailyDepartmentalLabourSlip.Handlers;

internal sealed class DailyDepartmentalLabourSlipHandlers :
    IRequestHandler<GetAllDailyDepartmentalLabourSlipsQuery, IEnumerable<DailyDepartmentalLabourSlipModel>>,
    IRequestHandler<GetDailyDepartmentalLabourSlipByIdQuery, DailyDepartmentalLabourSlipModel?>,
    IRequestHandler<CreateDailyDepartmentalLabourSlipCommand, DailyDepartmentalLabourSlipModel>,
    IRequestHandler<UpdateDailyDepartmentalLabourSlipCommand, DailyDepartmentalLabourSlipModel?>,
    IRequestHandler<DeleteDailyDepartmentalLabourSlipCommand, bool>,
    IRequestHandler<DeleteDDLSCommand, bool>,
    IRequestHandler<GetDailyDepartmentalLabourSlipsByProjectID, DataSet>
{
    private readonly IExecutionDbContext _db;
    private readonly Himapp.Execution.Contracts.References.IDdlSlipCodeGenerator _codeGenerator;
    private readonly Microsoft.Extensions.Logging.ILogger<DailyDepartmentalLabourSlipHandlers> _logger;
    public DailyDepartmentalLabourSlipHandlers(IExecutionDbContext db, Himapp.Execution.Contracts.References.IDdlSlipCodeGenerator codeGenerator, Microsoft.Extensions.Logging.ILogger<DailyDepartmentalLabourSlipHandlers>? logger = null) => (_db, _codeGenerator, _logger) = (db, codeGenerator, logger ?? Microsoft.Extensions.Logging.Abstractions.NullLogger<DailyDepartmentalLabourSlipHandlers>.Instance);

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
                d.DDLSlipCode,
                d.Remarks,
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
        var entity = await _db.Set<Himapp.Execution.Domain.Entities.DailyDepartmentalLabourSlip>().FirstOrDefaultAsync(a => a.ID == model.ProgramRowId, cancellationToken);

        if (entity is null) return false;

        // Mark child detail records active/inactive
        var details = await _db.Set<Himapp.Execution.Domain.Entities.DailyDepartmentalLabourSlipDetails>()
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

        return new DailyDepartmentalLabourSlipModel(d.ID, d.UniqueID, d.ProjectID, d.SlipDate, d.DDLSlipCode, d.Remarks, d.IsActive, d.CreatedBy, d.CreatedDate, d.LastModifiedBy, d.LastModifiedDate, details);
    }

    public async Task<DailyDepartmentalLabourSlipModel> Handle(CreateDailyDepartmentalLabourSlipCommand request, CancellationToken cancellationToken)
    {
        var r = request.Request;

        // Generate DDLSlipCode project-wise before constructing the entity so we can log and inspect it
        var generatedCode = await _codeGenerator.GenerateDDLSlipCodeAsync(r.ProjectId, cancellationToken);
        _logger.LogDebug("DDL slip code generated for ProjectId {ProjectId}: '{Code}'", r.ProjectId, generatedCode);

        var entity = new Himapp.Execution.Domain.Entities.DailyDepartmentalLabourSlip
        {
            UniqueID = Guid.NewGuid(),
            ProjectID = r.ProjectId,
            DDLSlipCode = string.IsNullOrWhiteSpace(generatedCode) ? null : generatedCode,
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
                    DDLSlipID = entity.ID,
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

        return new DailyDepartmentalLabourSlipModel(entity.ID, entity.UniqueID, entity.ProjectID, entity.SlipDate, entity.DDLSlipCode, entity.Remarks, entity.IsActive, entity.CreatedBy, entity.CreatedDate, entity.LastModifiedBy, entity.LastModifiedDate, details);
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

        return new DailyDepartmentalLabourSlipModel(entity.ID, entity.UniqueID, entity.ProjectID, entity.SlipDate, entity.DDLSlipCode, entity.Remarks, entity.IsActive, entity.CreatedBy, entity.CreatedDate, entity.LastModifiedBy, entity.LastModifiedDate, details);
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

    public async Task<DataSet> Handle(GetDailyDepartmentalLabourSlipsByProjectID request, CancellationToken cancellationToken)
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
        using (var cmd = new NpgsqlCommand("SELECT * FROM execution.uspgetdailydepartmentallabourslipbyprojectid(@p_projectid,@p_filtercolumn,@p_filtervalue,@p_pageindex,@p_pagesize,@p_sortcolumn,@p_isactive)", conn))
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
        using (var cmd2 = new NpgsqlCommand("SELECT cnt FROM execution.uspgetdailydepartmentallabourslipcountbyprojectid(@p_projectid,@p_filtercolumn,@p_filtervalue,@p_pageindex,@p_pagesize,@p_sortcolumn,@p_isactive)", conn))
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
