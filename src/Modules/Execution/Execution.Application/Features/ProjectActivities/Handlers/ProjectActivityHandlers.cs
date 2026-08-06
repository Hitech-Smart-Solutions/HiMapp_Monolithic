using Himapp.Execution.Application.Features.ProjectActivities.Commands;
using Himapp.Execution.Application.Features.ProjectActivities.Models;
using Himapp.Execution.Application.Features.ProjectActivities.Queries;
using Himapp.Execution.Contracts;
using Himapp.Execution.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using NpgsqlTypes;
using System.Data;

namespace Himapp.Execution.Application.Features.ProjectActivities.Handlers;

internal sealed class ProjectActivityHandlers :
    IRequestHandler<CreateProjectActivityCommand, ProjectActivityModel>,
    IRequestHandler<UpdateProjectActivityCommand, ProjectActivityModel?>,
    IRequestHandler<DeleteProjectActivityCommand, bool>,
    IRequestHandler<GetAllProjectActivitiesQuery, System.Data.DataSet>,
    IRequestHandler<GetProjectActivityByIdQuery, ProjectActivityModel?>,
    IRequestHandler<GetProjectActivitiesByProjectIdQuery, System.Data.DataSet>
{
    private readonly IExecutionDbContext _db;
    public ProjectActivityHandlers(IExecutionDbContext db) => _db = db;

    public async Task<ProjectActivityModel> Handle(CreateProjectActivityCommand request, CancellationToken cancellationToken)
    {
        var r = request.Request;
        var entityExists = await _db.Set<ProjectActivity>().FirstOrDefaultAsync(x => x.ProjectID == r.ProjectId && x.ActivityID == r.ActivityId, cancellationToken);
        if (entityExists is null)
        {
            var entity = new ProjectActivity
            {
                UniqueID = Guid.NewGuid(),
                ProjectID = r.ProjectId,
                ActivityID = r.ActivityId,
                Enabled = r.Enabled,
                RevenueRate = r.RevenueRate,
                SkilledLabourRate = r.SkilledLabourRate,
                UnSkilledLabourRate = r.UnSkilledLabourRate,
                OtherLabourRate = r.OtherLabourRate,
                OutputRequired = r.OutputRequired,
                IsActive = true,
                CreatedBy = r.CreatedBy,
                CreatedDate = DateTimeOffset.UtcNow,
                LastModifiedBy = r.LastModifiedBy,
                LastModifiedDate = DateTimeOffset.UtcNow
            };

            _db.Set<ProjectActivity>().Add(entity);
            await _db.SaveChangesAsync(cancellationToken);
            return new ProjectActivityModel(entity.ID, entity.UniqueID, entity.ProjectID, entity.ActivityID, entity.IsActive, entity.Enabled, entity.RevenueRate, entity.SkilledLabourRate, entity.UnSkilledLabourRate, entity.OtherLabourRate, entity.OutputRequired, entity.CreatedBy, entity.CreatedDate, entity.LastModifiedBy, entity.LastModifiedDate);
        }
        else
        {
            entityExists.ProjectID = request.Request.ProjectId;
            entityExists.ActivityID = request.Request.ActivityId;
            entityExists.Enabled = request.Request.Enabled;
            entityExists.RevenueRate = request.Request.RevenueRate;
            entityExists.SkilledLabourRate = request.Request.SkilledLabourRate;
            entityExists.UnSkilledLabourRate = request.Request.UnSkilledLabourRate;
            entityExists.OtherLabourRate = request.Request.OtherLabourRate;
            entityExists.OutputRequired = request.Request.OutputRequired;
            entityExists.LastModifiedDate = DateTimeOffset.UtcNow;

            await _db.SaveChangesAsync(cancellationToken);

            return new ProjectActivityModel(entityExists.ID, entityExists.UniqueID, entityExists.ProjectID, entityExists.ActivityID, entityExists.IsActive, entityExists.Enabled, entityExists.RevenueRate, entityExists.SkilledLabourRate, entityExists.UnSkilledLabourRate, entityExists.OtherLabourRate, entityExists.OutputRequired, entityExists.CreatedBy, entityExists.CreatedDate, entityExists.LastModifiedBy, entityExists.LastModifiedDate);
        }

    }

    public async Task<ProjectActivityModel?> Handle(UpdateProjectActivityCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.Set<ProjectActivity>().FirstOrDefaultAsync(x => x.ID == request.Id, cancellationToken);
        if (entity is null) return null;

        entity.ProjectID = request.Request.ProjectId;
        entity.ActivityID = request.Request.ActivityId;
        entity.Enabled = request.Request.Enabled;
        entity.RevenueRate = request.Request.RevenueRate;
        entity.SkilledLabourRate = request.Request.SkilledLabourRate;
        entity.UnSkilledLabourRate = request.Request.UnSkilledLabourRate;
        entity.OtherLabourRate = request.Request.OtherLabourRate;
        entity.OutputRequired = request.Request.OutputRequired;
        entity.LastModifiedDate = DateTimeOffset.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);

        return new ProjectActivityModel(entity.ID, entity.UniqueID, entity.ProjectID, entity.ActivityID, entity.IsActive, entity.Enabled, entity.RevenueRate, entity.SkilledLabourRate, entity.UnSkilledLabourRate, entity.OtherLabourRate, entity.OutputRequired, entity.CreatedBy, entity.CreatedDate, entity.LastModifiedBy, entity.LastModifiedDate);
    }

    public async Task<bool> Handle(DeleteProjectActivityCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.Set<ProjectActivity>().FirstOrDefaultAsync(x => x.ActivityID == request.Id && x.ProjectID == request.ProjectId, cancellationToken);
        if (entity is null) return false;
        _db.Set<ProjectActivity>().Remove(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<System.Data.DataSet> Handle(GetAllProjectActivitiesQuery request, CancellationToken cancellationToken)
    {
        var p = request.SearchParams ?? new SearchParamsCompanyProjectWise();

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
        using (var cmd = new NpgsqlCommand("SELECT * FROM execution.uuspgetexecutionactivitiesforactivitymapping(@p_companyid,@p_projectid,@p_filtercolumn,@p_filtervalue,@p_pageindex,@p_pagesize,@p_sortcolumn,@p_isactive)", conn))
        {
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = 1800;
            cmd.Parameters.AddWithValue("@p_companyid", NpgsqlDbType.Integer, p.CompanyID);
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
        using (var cmd2 = new NpgsqlCommand("SELECT cnt FROM execution.uspgetexecutionactivitiescountforactivitymapping(@p_companyid,@p_projectid,@p_filtercolumn,@p_filtervalue,@p_pageindex,@p_pagesize,@p_sortcolumn,@p_isactive)", conn))
        {
            cmd2.CommandType = CommandType.Text;
            cmd2.CommandTimeout = 1800;
            cmd2.Parameters.AddWithValue("@p_companyid", NpgsqlDbType.Integer, p.CompanyID);
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

    public async Task<ProjectActivityModel?> Handle(GetProjectActivityByIdQuery request, CancellationToken cancellationToken)
    {
        var p = await _db.Set<ProjectActivity>().AsNoTracking().FirstOrDefaultAsync(x => x.ID == request.Id, cancellationToken);
        if (p is null) return null;
        return new ProjectActivityModel(p.ID, p.UniqueID, p.ProjectID, p.ActivityID, p.IsActive, p.Enabled, p.RevenueRate, p.SkilledLabourRate, p.UnSkilledLabourRate, p.OtherLabourRate, p.OutputRequired, p.CreatedBy, p.CreatedDate, p.LastModifiedBy, p.LastModifiedDate);
    }

    public async Task<System.Data.DataSet> Handle(GetProjectActivitiesByProjectIdQuery request, CancellationToken cancellationToken)
    {
       
        // Prepare DataSet
        var ds = new System.Data.DataSet("ProjectActivitiesResult");

        // Force Npgsql path: require the underlying DbContext to obtain connection string
        var dbContext = _db as DbContext;
        if (dbContext is null)
            throw new InvalidOperationException("IExecutionDbContext is not a DbContext. Cannot obtain connection string for Npgsql operations.");

        var dsLocal = new DataSet("ProjectActivitiesResult");
        var connString = dbContext.Database.GetDbConnection().ConnectionString;

        using var conn = new NpgsqlConnection(connString);
        await conn.OpenAsync(cancellationToken);

        // Rows table
        using (var cmd = new NpgsqlCommand("SELECT * FROM execution.uspgetprojectactivitiesbyprojectid(@p_projectid)", conn))
        {
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = 1800;
            cmd.Parameters.AddWithValue("@p_projectid", NpgsqlDbType.Integer, request.ProjectId);

            var da = new NpgsqlDataAdapter(cmd);
            var dt = new DataTable("Rows");
            da.Fill(dt);
            dsLocal.Tables.Add(dt);
        }

        return dsLocal;
    }
}

