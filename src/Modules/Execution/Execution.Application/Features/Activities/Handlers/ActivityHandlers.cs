using Himapp.Execution.Application.Features.Activities.Commands;
using Himapp.Execution.Application.Features.Activities.Queries;
using Himapp.Execution.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using System.Data;
using System.Linq;
using System.Collections.Generic;
using System;
using Npgsql;
using NpgsqlTypes;
using Himapp.Execution.Application.Features.Activities.Models;

namespace Himapp.Execution.Application.Features.Activities.Handlers;

internal sealed class ActivityHandlers :
    IRequestHandler<CreateActivityCommand, ActivityDto>,
    IRequestHandler<UpdateActivityCommand, ActivityDto?>,
    IRequestHandler<DeleteActivityCommand, bool>,
    IRequestHandler<GetAllActivitiesQuery, System.Data.DataSet>,
    IRequestHandler<GetActivityByIdQuery, ActivityDto?>
{
    private readonly IExecutionDbContext _db;
    public ActivityHandlers(IExecutionDbContext db) => _db = db;

    public async Task<ActivityDto> Handle(CreateActivityCommand request, CancellationToken cancellationToken)
    {
        var entity = new Activity
        {
            UniqueID = Guid.NewGuid(),
            CompanyID = request.CompanyID,
            ActivityName = request.ActivityName,
            UOMID = request.UOMID,
            RevenueRate = request.RevenueRate,
            SkilledLabourRate = request.SkilledLabourRate,
            UnSkilledLabourRate = request.UnSkilledLabourRate,
            OtherLabourRate = request.OtherLabourRate,
            OutputRequired = request.OutputRequired,
            IsActive = true,
            CreatedBy = request.CreateBy,
            CreatedDate = DateTimeOffset.UtcNow,
            LastModifiedBy = request.LastModifiedBy,
            LastModifiedDate = DateTimeOffset.UtcNow
        };

        _db.Set<Activity>().Add(entity);
        await _db.SaveChangesAsync(cancellationToken);

        return new ActivityDto(entity.ID, request.CompanyID, request.ActivityName, request.UOMID, request.RevenueRate, request.SkilledLabourRate, request.UnSkilledLabourRate, request.OtherLabourRate, request.OutputRequired, request.CreateBy, request.LastModifiedBy);
    }

    public async Task<ActivityDto?> Handle(UpdateActivityCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.Set<Activity>().FirstOrDefaultAsync(a => a.ID == request.Id, cancellationToken);
        if (entity is null) return null;

        entity.ActivityName = request.ActivityName;
        entity.UOMID = request.UOMID;
        entity.IsActive = true;
        entity.RevenueRate = request.RevenueRate;
        entity.SkilledLabourRate = request.SkilledLabourRate;
        entity.UnSkilledLabourRate = request.UnSkilledLabourRate;
        entity.OtherLabourRate = request.OtherLabourRate;
        entity.OutputRequired = request.OutputRequired;
        entity.LastModifiedBy = request.LastModifiedBy;
        entity.LastModifiedDate = DateTimeOffset.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);

        return new ActivityDto(entity.ID, entity.CompanyID, entity.ActivityName, entity.UOMID, request.RevenueRate, request.SkilledLabourRate, request.UnSkilledLabourRate, request.OtherLabourRate, request.OutputRequired, entity.CreatedBy, entity.LastModifiedBy);
    }

    public async Task<bool> Handle(DeleteActivityCommand request, CancellationToken cancellationToken)
    {
        var model = request.addTransactionActionHistoryDTO;
        var entity = await _db.Set<Activity>().FirstOrDefaultAsync(a => a.ID == model.ProgramRowId, cancellationToken);

        if (entity is null) return false;

        // Mark child records inactive
        var pas = await _db.Set<ProjectActivity>()
            .Where(x => x.ActivityID == model.ProgramRowId)
            .ToListAsync(cancellationToken);

        foreach (var pa in pas)
        {
            pa.IsActive = model.Actions == Actions.Activated ? true : false; // or pa.Enabled = false;
        }

        // Mark main entity inactive
        entity.IsActive = model.Actions == Actions.Activated ? true : false; // or entity.Enabled = false;

        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<System.Data.DataSet> Handle(GetAllActivitiesQuery request, CancellationToken cancellationToken)
    {
        var p = request.SearchParams ?? new SearchParams();

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
        using (var cmd = new NpgsqlCommand("SELECT * FROM execution.uspgetexecutionactivitiesbycompanyid(@p_companyid,@p_filtercolumn,@p_filtervalue,@p_pageindex,@p_pagesize,@p_sortcolumn,@p_isactive)", conn))
        {
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = 1800;
            cmd.Parameters.AddWithValue("@p_companyid", NpgsqlDbType.Integer, p.CompanyID);
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
        using (var cmd2 = new NpgsqlCommand("SELECT cnt FROM execution.uspgetexecutionactivitiescountbycompanyid(@p_companyid,@p_filtercolumn,@p_filtervalue,@p_pageindex,@p_pagesize,@p_sortcolumn,@p_isactive)", conn))
        {
            cmd2.CommandType = CommandType.Text;
            cmd2.CommandTimeout = 1800;
            cmd2.Parameters.AddWithValue("@p_companyid", NpgsqlDbType.Integer, p.CompanyID);
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

    public async Task<ActivityDto?> Handle(GetActivityByIdQuery request, CancellationToken cancellationToken)
    {
        var activities = _db.Set<Activity>().AsNoTracking();

        var dto = await (from a in activities
                         where a.ID == request.Id
                         select new ActivityDto(
                             a.ID,
                             a.CompanyID,
                             a.ActivityName,
                             a.UOMID,
                             a.RevenueRate,
                             a.SkilledLabourRate,
                             a.UnSkilledLabourRate,
                             a.OtherLabourRate,
                             a.OutputRequired,                             
                             a.CreatedBy,
                             a.LastModifiedBy))
                        .FirstOrDefaultAsync(cancellationToken);

        return dto;
    }

    // removed AddParameter helper; using NpgsqlDataAdapter and AddWithValue for parameter handling
}

