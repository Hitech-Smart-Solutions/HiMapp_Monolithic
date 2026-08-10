using Himapp.Execution.Application.Features.DailyLabor.Commands;
using Himapp.Execution.Application.Features.DailyLabor.Models;
using Himapp.Execution.Application.Features.DailyLabor.Queries;
using Himapp.Execution.Application.Features.Manpower.Queries;
using Himapp.Execution.Contracts;
using Himapp.Execution.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using Npgsql;
using NpgsqlTypes;
using System.Data;

namespace Himapp.Execution.Application.Features.DailyLabor.Handlers;

internal sealed class DailyLaborHandlers :
    IRequestHandler<GetAllDailyLaborsQuery, IReadOnlyCollection<DailyLaborModel>>,
    IRequestHandler<GetDailyLaborByIdQuery, DailyLaborModel?>,
    IRequestHandler<CreateDailyLaborCommand, DailyLaborModel>,
    IRequestHandler<UpdateDailyLaborCommand, DailyLaborModel?>,
    IRequestHandler<DeleteDailyLaborCommand, bool>,
    IRequestHandler<GetConsolidatedDailyLaborQuery, IReadOnlyCollection<DailyLaborConsolidatedModel>>,
    IRequestHandler<GetDailyLaborByProjectID, DataSet>
{
    private readonly IExecutionDbContext _db;
    public DailyLaborHandlers(IExecutionDbContext db) => _db = db;

    public async Task<IReadOnlyCollection<DailyLaborModel>> Handle(GetAllDailyLaborsQuery request, CancellationToken cancellationToken)
    {
        // Return header-only projection for performance (details omitted)
        return await _db.Set<Himapp.Execution.Domain.Entities.DailyLabor>()
            .AsNoTracking()
            .Where(d => d.IsActive)
            .Select(d => new DailyLaborModel(
                d.ID,
                d.UniqueID,
                d.CompanyID,
                d.ProjectID,
                d.DLRDate,
                d.Remarks,
                d.StateID,
                d.IsActive,
                d.CreatedBy,
                d.CreatedDate,
                d.LastModifiedBy,
                d.LastModifiedDate,
                Array.Empty<DailyLaborDetailModel>()))
            .ToArrayAsync(cancellationToken);
    }

    public async Task<DailyLaborModel?> Handle(GetDailyLaborByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _db.Set<Himapp.Execution.Domain.Entities.DailyLabor>()
            .AsNoTracking()
            .Include(d => d.DailyLaborDetail)
            .FirstOrDefaultAsync(x => x.ID == request.Id && x.IsActive, cancellationToken);

        if (entity is null) return null;

        var details = entity.DailyLaborDetail?.Select(dd => new DailyLaborDetailModel(
            dd.ID,
            dd.UniqueID,
            dd.ContractorID,
            dd.CategoryID,
            dd.Skilled,
            dd.UnSkilled,
            dd.Remarks,
            dd.Mat,
            dd.ContractorName,
            dd.ActivityID)).ToArray()
            ?? Array.Empty<DailyLaborDetailModel>();

        return new DailyLaborModel(
            entity.ID,
            entity.UniqueID,
            entity.CompanyID,
            entity.ProjectID,
            entity.DLRDate,
            entity.Remarks,
            entity.StateID,
            entity.IsActive,
            entity.CreatedBy,
            entity.CreatedDate,
            entity.LastModifiedBy,
            entity.LastModifiedDate,
            details);
    }

    public async Task<DailyLaborModel> Handle(CreateDailyLaborCommand request, CancellationToken cancellationToken)
    {
        var r = request.Request;

        var entity = new Himapp.Execution.Domain.Entities.DailyLabor
        {
            UniqueID = Guid.NewGuid(),
            ProjectID = r.ProjectId,
            DLRDate = DateTime.SpecifyKind(r.ReportDate, DateTimeKind.Utc),
            Remarks = r.Remarks,
            StateID = (short?)r.Status,
            IsActive = true,
            CreatedBy = 0,
            CreatedDate = DateTime.UtcNow,
            LastModifiedBy = 0,
            LastModifiedDate = DateTime.UtcNow
        };

        if (r.Details?.Any() == true)
        {
            foreach (var d in r.Details)
            {
                var detail = new Himapp.Execution.Domain.Entities.DailyLaborDetail
                {
                    UniqueID = Guid.NewGuid(),
                    ContractorID = d.ContractorId,
                    CategoryID = d.CategoryId,
                    Skilled = d.Skilled,
                    UnSkilled = d.UnSkilled,
                    Remarks = d.Remarks,
                    Mat = d.Mat,
                    ContractorName = d.ContractorName,
                    ActivityID = d.ActivityId,
                    IsActive = true,
                    CreatedBy = 0,
                    CreatedDate = DateTimeOffset.UtcNow,
                    LastModifiedBy = 0,
                    LastModifiedDate = DateTimeOffset.UtcNow,
                    DailyLabor = entity
                };

                entity.DailyLaborDetail?.Add(detail);
            }
        }

        _db.Set<Himapp.Execution.Domain.Entities.DailyLabor>().Add(entity);
        await _db.SaveChangesAsync(cancellationToken);

        var details = entity.DailyLaborDetail?.Select(dd => new DailyLaborDetailModel(dd.ID, dd.UniqueID, dd.ContractorID, dd.CategoryID, dd.Skilled, dd.UnSkilled, dd.Remarks, dd.Mat, dd.ContractorName, dd.ActivityID)).ToArray() ?? Array.Empty<DailyLaborDetailModel>();

        return new DailyLaborModel(entity.ID, entity.UniqueID, entity.CompanyID, entity.ProjectID, entity.DLRDate, entity.Remarks, entity.StateID, entity.IsActive, entity.CreatedBy, entity.CreatedDate, entity.LastModifiedBy, entity.LastModifiedDate, details);
    }
    public async Task<bool> Handle(DeleteDailyLaborCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.Set<Himapp.Execution.Domain.Entities.DailyLabor>().Include(d => d.DailyLaborDetail).FirstOrDefaultAsync(x => x.ID == request.Id && x.IsActive, cancellationToken);
        if (entity is null) return false;

        // Soft delete header and child details
        entity.IsActive = false;
        entity.LastModifiedBy = 0;
        entity.LastModifiedDate = DateTime.UtcNow;

        if (entity.DailyLaborDetail != null)
        {
            foreach (var dd in entity.DailyLaborDetail)
            {
                dd.IsActive = false;
                dd.LastModifiedBy = 0;
                dd.LastModifiedDate = DateTimeOffset.UtcNow;
            }
        }

        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<DailyLaborModel?> Handle(UpdateDailyLaborCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.Set<Himapp.Execution.Domain.Entities.DailyLabor>()
            .Include(d => d.DailyLaborDetail)
            .FirstOrDefaultAsync(x => x.ID == request.Id && x.IsActive, cancellationToken);

        if (entity is null) return null;

        var r = request.Request;

        entity.ProjectID = r.ProjectId;
        entity.DLRDate = DateTime.SpecifyKind(r.ReportDate,DateTimeKind.Utc);
        entity.Remarks = r.Remarks;
        entity.StateID = (short?)r.Status;
        entity.LastModifiedDate = DateTime.UtcNow;

        // Remove existing details (physically) and add new ones
        if (entity.DailyLaborDetail != null && entity.DailyLaborDetail.Any())
        {
            _db.Set<Himapp.Execution.Domain.Entities.DailyLaborDetail>().RemoveRange(entity.DailyLaborDetail);
            entity.DailyLaborDetail.Clear();
        }

        if (r.Details?.Any() == true)
        {
            foreach (var d in r.Details)
            {
                var detail = new Himapp.Execution.Domain.Entities.DailyLaborDetail
                {
                    UniqueID = Guid.NewGuid(),
                    ContractorID = d.ContractorId,
                    CategoryID = d.CategoryId,
                    Skilled = d.Skilled,
                    UnSkilled = d.UnSkilled,
                    Remarks = d.Remarks,
                    Mat = d.Mat,
                    ContractorName = d.ContractorName,
                    ActivityID = d.ActivityId,
                    IsActive = true,
                    CreatedBy = 0,
                    CreatedDate = DateTimeOffset.UtcNow,
                    LastModifiedBy = 0,
                    LastModifiedDate = DateTimeOffset.UtcNow,
                    DailyLabor = entity
                };

                entity.DailyLaborDetail?.Add(detail);
            }
        }

        await _db.SaveChangesAsync(cancellationToken);

        var details = entity.DailyLaborDetail?.Select(dd => new DailyLaborDetailModel(dd.ID, dd.UniqueID, dd.ContractorID, dd.CategoryID, dd.Skilled, dd.UnSkilled, dd.Remarks, dd.Mat, dd.ContractorName, dd.ActivityID)).ToArray() ?? Array.Empty<DailyLaborDetailModel>();

        return new DailyLaborModel(entity.ID, entity.UniqueID, entity.CompanyID, entity.ProjectID, entity.DLRDate, entity.Remarks, entity.StateID, entity.IsActive, entity.CreatedBy, entity.CreatedDate, entity.LastModifiedBy, entity.LastModifiedDate, details);
    }

    public async Task<IReadOnlyCollection<DailyLaborConsolidatedModel>> Handle(GetConsolidatedDailyLaborQuery request, CancellationToken cancellationToken)
    {
        var result = await _db.Set<Himapp.Execution.Domain.Entities.Manpower>()
            .AsNoTracking()
            .Where(m =>
                m.ProjectID == request.ProjectId &&
                m.EntryDate == request.Date &&
                m.IsActive)
            .SelectMany(m => m.ManpowerDetail!
                .Where(md => md.IsActive)
                .Select(md => new
                {
                    md.ContractorID,
                    md.ActivityID,
                    md.SkilledCount,
                    md.UnskilledCount,
                    md.OtherCount
                }))
            .GroupBy(x => new
            {
                x.ContractorID,
                x.ActivityID
            })
            .Select(g => new DailyLaborConsolidatedModel(
                g.Key.ContractorID,
                g.Key.ActivityID,
                g.Sum(x => x.SkilledCount),
                g.Sum(x => x.UnskilledCount),
                g.Sum(x => x.OtherCount),
                g.Sum(x =>
                    x.SkilledCount +
                    x.UnskilledCount +
                    x.OtherCount)
            ))
            .ToArrayAsync(cancellationToken);

        return result;
    }

    public async Task<DataSet> Handle(GetDailyLaborByProjectID request, CancellationToken cancellationToken)
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
        using (var cmd = new NpgsqlCommand("SELECT * FROM execution.uspgetdailylaborbyprojectid(@p_projectid,@p_filtercolumn,@p_filtervalue,@p_pageindex,@p_pagesize,@p_sortcolumn,@p_isactive)", conn))
        {
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = 1800;
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
        using (var cmd2 = new NpgsqlCommand("SELECT cnt FROM execution.uspgetdailylaborcountbyprojectid(@p_projectid,@p_filtercolumn,@p_filtervalue,@p_pageindex,@p_pagesize,@p_sortcolumn,@p_isactive)", conn))
        {
            cmd2.CommandType = CommandType.Text;
            cmd2.CommandTimeout = 1800;
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

