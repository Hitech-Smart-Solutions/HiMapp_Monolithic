using DocumentFormat.OpenXml.InkML;
using Himapp.Admin.Contracts.Projects;
using Himapp.Execution.Application.Features.DailyLabor.Commands;
using Himapp.Execution.Application.Features.DailyLabor.Models;
using Himapp.Execution.Application.Features.DailyLabor.Queries;
using Himapp.Execution.Application.Features.DailyProgress.Models;
using Himapp.Execution.Application.Features.DailyProgress.Queries;
using Himapp.Execution.Application.Features.Manpower.Models;
using Himapp.Execution.Application.Features.Manpower.Queries;
using Himapp.Execution.Contracts;
using Himapp.Execution.Domain.Entities;
using Himapp.Execution.Contracts.DailyLabor;
using Himapp.SharedKernel.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using Npgsql;
using NpgsqlTypes;
using System.Data;
using DailyLaborEntity = Himapp.Execution.Domain.Entities.DailyLabor;
using ManpowerEntity = Himapp.Execution.Domain.Entities.Manpower;

namespace Himapp.Execution.Application.Features.DailyLabor.Handlers;

internal sealed class DailyLaborHandlers :
    IRequestHandler<GetAllDailyLaborsQuery, IReadOnlyCollection<DailyLaborModel>>,
    IRequestHandler<GetDailyLaborByIdQuery, DailyLaborModel?>,
    IRequestHandler<CreateDailyLaborCommand, DailyLaborModel>,
    IRequestHandler<UpdateDailyLaborCommand, DailyLaborModel?>,
    IRequestHandler<DeleteDailyLaborCommand, bool>,
    IRequestHandler<DeleteDailyLaborActionCommand, bool>,
    IRequestHandler<GetConsolidatedDailyLaborQuery, IReadOnlyCollection<DailyLaborConsolidatedModel>>,
    IRequestHandler<GetDailyLaborByProjectID, DataSet>,
    IRequestHandler<DPRGetConsolidatedDailyLaborQuery, IReadOnlyCollection<DPRDailyLaborConsolidatedModel>>
{
    private readonly IExecutionDbContext _db;
    private readonly IProjectDirectory _projectDirectory;
    private readonly Himapp.Execution.Contracts.References.IDlrCodeGenerator _codeGenerator;
    private readonly ICurrentUser _currentUser;
    public DailyLaborHandlers(IExecutionDbContext db, IProjectDirectory projectDirectory, Himapp.Execution.Contracts.References.IDlrCodeGenerator codeGenerator, ICurrentUser currentUser) => (_db, _projectDirectory, _codeGenerator, _currentUser) = (db, projectDirectory, codeGenerator, currentUser);

    private int CurrentUserId => _currentUser.UserId ?? throw new UnauthorizedAccessException("An authenticated user is required.");

    public async Task<IReadOnlyCollection<DailyLaborModel>> Handle(GetAllDailyLaborsQuery request, CancellationToken cancellationToken)
    {
        // Return header-only projection for performance (details omitted)
        return await _db.Set<DailyLaborEntity>()
            .AsNoTracking()
            .Where(d => d.IsActive)
            .Select(d => new DailyLaborModel(
                d.ID,
                d.UniqueID,
                d.DLRCode,
                d.CompanyID,
                d.ProjectID,
                d.DLRDate,
                d.Remarks,
                d.ProposedActionPlan,
                d.ConstraintsAndReasons,
                d.RemoveMenPower,
                d.StateID,
                d.IsActive,
                d.CreatedBy,
                d.CreatedDate,
                d.LastModifiedBy,
                d.LastModifiedDate,
                Array.Empty<DailyLaborDetailModel>()))
            .ToArrayAsync(cancellationToken);
    }

    public async Task<bool> Handle(DeleteDailyLaborActionCommand request, CancellationToken cancellationToken)
    {
        var model = request.addTransactionActionHistoryDTO;
        var entity = await _db.Set<DailyLaborEntity>().FirstOrDefaultAsync(a => a.ID == model.ProgramRowId, cancellationToken);

        if (entity is null) return false;

        // Mark child detail records active/inactive
        var details = await _db.Set<DailyLaborDetail>()
            .Where(x => x.DailyLabourID == model.ProgramRowId)
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

    public async Task<DailyLaborModel?> Handle(GetDailyLaborByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _db.Set<DailyLaborEntity>()
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
            dd.ActivityID,
            string.Empty)).ToArray()
            ?? Array.Empty<DailyLaborDetailModel>();

        return new DailyLaborModel(
            entity.ID,
            entity.UniqueID,
            entity.DLRCode,
            entity.CompanyID,
            entity.ProjectID,
            entity.DLRDate,
            entity.Remarks,
            entity.ProposedActionPlan,
            entity.ConstraintsAndReasons,
            entity.RemoveMenPower,
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
        var userId = CurrentUserId;

        if (!Enum.IsDefined(typeof(DailyLaborState), (short)r.Status))
        {
            throw new ArgumentException("Invalid Daily Labor status.");
        }

        var entity = new DailyLaborEntity
        {
            UniqueID = Guid.NewGuid(),
            ProjectID = r.ProjectId,
            ConstraintsAndReasons = r.ConstraintsAndReasons,
            ProposedActionPlan = r.ProposedActionPlan,
            RemoveMenPower = r.RemoveMenPower,
            CompanyID = r.CompanyID,
            DLRDate = DateTime.SpecifyKind(r.ReportDate, DateTimeKind.Utc),
            Remarks = r.Remarks,
            StateID = (short)r.Status,
            IsActive = true,
            CreatedBy = userId,
            CreatedDate = DateTime.UtcNow,
            LastModifiedBy = userId,
            LastModifiedDate = DateTime.UtcNow
        };

        // Generate DLRCode using shared DLR code generator service
        var projectId = r.ProjectId;
        var generatedCode = await _codeGenerator.GenerateDLRCodeAsync(projectId, cancellationToken);
        entity.DLRCode = string.IsNullOrWhiteSpace(generatedCode) ? null : generatedCode;

        // Add details (if any)
        if (r.Details?.Any() == true)
        {
            foreach (var d in r.Details)
            {
                var detail = new DailyLaborDetail
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
                    CreatedBy = userId,
                    CreatedDate = DateTime.UtcNow,
                    LastModifiedBy = userId,
                    LastModifiedDate = DateTime.UtcNow,
                    DailyLabor = entity
                };

                entity.DailyLaborDetail?.Add(detail);
            }
        }

        _db.Set<DailyLaborEntity>().Add(entity);
        await _db.SaveChangesAsync(cancellationToken);

        var details = entity.DailyLaborDetail?.Select(dd => new DailyLaborDetailModel(dd.ID, dd.UniqueID, dd.ContractorID, dd.CategoryID, dd.Skilled, dd.UnSkilled, dd.Remarks, dd.Mat, dd.ContractorName, dd.ActivityID, string.Empty)).ToArray() ?? Array.Empty<DailyLaborDetailModel>();

        return new DailyLaborModel(entity.ID, entity.UniqueID, entity.DLRCode, entity.CompanyID, entity.ProjectID, entity.DLRDate, entity.Remarks, entity.ProposedActionPlan, entity.ConstraintsAndReasons, entity.RemoveMenPower, entity.StateID, entity.IsActive, entity.CreatedBy, entity.CreatedDate, entity.LastModifiedBy, entity.LastModifiedDate, details);
    }
    public async Task<bool> Handle(DeleteDailyLaborCommand request, CancellationToken cancellationToken)
    {
        var userId = CurrentUserId;
        var entity = await _db.Set<DailyLaborEntity>().Include(d => d.DailyLaborDetail).FirstOrDefaultAsync(x => x.ID == request.Id && x.IsActive, cancellationToken);
        if (entity is null) return false;

        // Soft delete header and child details
        entity.IsActive = false;
        entity.LastModifiedBy = userId;
        entity.LastModifiedDate = DateTime.UtcNow;

        if (entity.DailyLaborDetail != null)
        {
            foreach (var dd in entity.DailyLaborDetail)
            {
                dd.IsActive = false;
                dd.LastModifiedBy = userId;
                dd.LastModifiedDate = DateTime.UtcNow;
            }
        }

        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }
    
    public async Task<DailyLaborModel?> Handle(UpdateDailyLaborCommand request, CancellationToken cancellationToken)
    {
        var userId = CurrentUserId;
        var entity = await _db.Set<DailyLaborEntity>()
            .Include(d => d.DailyLaborDetail)
            .FirstOrDefaultAsync(x => x.ID == request.Id && x.IsActive, cancellationToken);

        if (entity is null) return null;

        var r = request.Request;

        if (!Enum.IsDefined(typeof(DailyLaborState), (short)r.Status))
        {
            throw new ArgumentException("Invalid Daily Labor status.");
        }

        if (entity.StateID == (short)DailyLaborState.Submitted)
        {
            throw new InvalidOperationException(
                "Submitted Daily Labor cannot be modified.");
        }

        entity.ProjectID = r.ProjectId;
        entity.DLRDate = DateTime.SpecifyKind(r.ReportDate, DateTimeKind.Utc);
        entity.Remarks = r.Remarks;
        entity.RemoveMenPower = r.RemoveMenPower;
        entity.StateID = (short?)r.Status;
        entity.LastModifiedBy = userId;
        entity.LastModifiedDate = DateTime.UtcNow;

        // Remove existing details (physically) and add new ones
        if (entity.DailyLaborDetail != null && entity.DailyLaborDetail.Any())
        {
            _db.Set<DailyLaborDetail>().RemoveRange(entity.DailyLaborDetail);
            entity.DailyLaborDetail.Clear();
        }

        if (r.Details?.Any() == true)
        {
            foreach (var d in r.Details)
            {
                var detail = new DailyLaborDetail
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
                    CreatedBy = userId,
                    CreatedDate = DateTime.UtcNow,
                    LastModifiedBy = userId,
                    LastModifiedDate = DateTime.UtcNow,
                    DailyLabor = entity
                };

                entity.DailyLaborDetail?.Add(detail);
            }
        }

        await _db.SaveChangesAsync(cancellationToken);

        var details = entity.DailyLaborDetail?.Select(dd => new DailyLaborDetailModel(dd.ID, dd.UniqueID, dd.ContractorID, dd.CategoryID, dd.Skilled, dd.UnSkilled, dd.Remarks, dd.Mat, dd.ContractorName, dd.ActivityID, string.Empty)).ToArray() ?? Array.Empty<DailyLaborDetailModel>();

        return new DailyLaborModel(entity.ID, entity.UniqueID, entity.DLRCode, entity.CompanyID, entity.ProjectID, entity.DLRDate, entity.Remarks, entity.ProposedActionPlan, entity.ConstraintsAndReasons, entity.RemoveMenPower, entity.StateID, entity.IsActive, entity.CreatedBy, entity.CreatedDate, entity.LastModifiedBy, entity.LastModifiedDate, details);
    }

    public async Task<IReadOnlyCollection<DailyLaborConsolidatedModel>> Handle(GetConsolidatedDailyLaborQuery request, CancellationToken cancellationToken)
    {
        var result = await _db.Set<ManpowerEntity>()
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
        using (var cmd2 = new NpgsqlCommand("SELECT cnt FROM execution.uspgetdailylaborcountbyprojectid(@p_projectid,@p_filtercolumn,@p_filtervalue,@p_pageindex,@p_pagesize,@p_sortcolumn,@p_isactive)", conn))
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

    public async Task<IReadOnlyCollection<DPRDailyLaborConsolidatedModel>> Handle(DPRGetConsolidatedDailyLaborQuery request, CancellationToken cancellationToken)
    {
        var result = await (
            from dl in _db.Set<Domain.Entities.DailyLabor>()
            .AsNoTracking()
            from d in dl.DailyLaborDetail!

            join a in _db.Set<Domain.Entities.Activity>()
                on d.ActivityID equals a.ID

            where
                DateOnly.FromDateTime(dl.DLRDate) == request.Date &&
                dl.ProjectID == request.ProjectId &&
                dl.IsActive &&
                d.IsActive

            group new { d, a } by new
            {
                d.ContractorID,
                d.ContractorName,
                d.ActivityID,
                a.ActivityName
            }
            into g

            select new DPRDailyLaborConsolidatedModel(
                g.Key.ContractorID,
                g.Key.ContractorName,
                g.Key.ActivityID,
                g.Key.ActivityName,
                g.Sum(x => x.d.Skilled ?? 0),
                g.Sum(x => x.d.UnSkilled ?? 0),
                g.Sum(x => x.d.Mat ?? 0),
                g.Sum(x =>
                    (x.d.Skilled ?? 0) +
                    (x.d.UnSkilled ?? 0) +
                    (x.d.Mat ?? 0))
            )
        ).ToArrayAsync(cancellationToken);

        return result;
    }

}
