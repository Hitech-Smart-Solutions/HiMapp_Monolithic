using DocumentFormat.OpenXml.InkML;
using Himapp.Api.src.Shared.Exceptions;
using Himapp.Execution.Application.Features.ProjectActivities.Commands;
using Himapp.Execution.Application.Features.ProjectActivities.Models;
using Himapp.Execution.Application.Features.ProjectActivities.Queries;
using Himapp.Execution.Contracts;
using Himapp.Execution.Domain.Entities;
using Himapp.SharedKernel.Abstractions;
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
    IRequestHandler<GetProjectActivitiesByProjectIdQuery, System.Data.DataSet>,
    IRequestHandler<GetProjectActivitiyDetailsByProjectID, List<ProjectActivityCategoryDetailsModel>>
{
    private readonly IExecutionDbContext _db;
    private readonly ICurrentUser _currentUser;
    public ProjectActivityHandlers(IExecutionDbContext db, ICurrentUser currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    private int CurrentUserId => _currentUser.UserId ?? throw new UnauthorizedAccessException("An authenticated user is required.");

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
                CreatedBy = r.CreatedBy ?? 0,
                CreatedDate = DateTime.UtcNow,
                LastModifiedBy = r.LastModifiedBy ?? 0,
                LastModifiedDate = DateTime.UtcNow
            };

            _db.Set<ProjectActivity>().Add(entity);
            await AddActivityCategoryDetailsAsync(entity, r.SkilledLabourRate, r.UnSkilledLabourRate, r.OtherLabourRate,
                r.LastModifiedBy, cancellationToken);
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
            entityExists.LastModifiedBy = request.Request.LastModifiedBy ?? 0;
            entityExists.LastModifiedDate = DateTime.UtcNow;

            await UpdateActivityCategoryDetailsAsync(entityExists, request.Request.SkilledLabourRate,
                request.Request.UnSkilledLabourRate, request.Request.OtherLabourRate,
                request.Request.LastModifiedBy, cancellationToken);

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
        entity.LastModifiedBy = request.Request.LastModifiedBy ?? 0;
        entity.LastModifiedDate = DateTime.UtcNow;

        await UpdateActivityCategoryDetailsAsync(entity, request.Request.SkilledLabourRate,
            request.Request.UnSkilledLabourRate, request.Request.OtherLabourRate,
            request.Request.LastModifiedBy, cancellationToken);

        await _db.SaveChangesAsync(cancellationToken);

        return new ProjectActivityModel(entity.ID, entity.UniqueID, entity.ProjectID, entity.ActivityID, entity.IsActive, entity.Enabled, entity.RevenueRate, entity.SkilledLabourRate, entity.UnSkilledLabourRate, entity.OtherLabourRate, entity.OutputRequired, entity.CreatedBy, entity.CreatedDate, entity.LastModifiedBy, entity.LastModifiedDate);
    }

    private async Task AddActivityCategoryDetailsAsync(ProjectActivity projectActivity, decimal skilledRate, decimal unskilledRate, decimal otherRate, int? lastModifiedBy, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var activityName = await GetActivityNameAsync(projectActivity.ActivityID, cancellationToken);
        var userId = CurrentUserId;

        foreach (var (categoryType, rate) in GetCategoryRates(skilledRate, unskilledRate, otherRate))
        {
            _db.Set<ActivityCategoryDetails>().Add(new ActivityCategoryDetails
            {
                UniqueID = Guid.NewGuid(),
                ProjectID = projectActivity.ProjectID,
                ActivityID = projectActivity.ActivityID,
                CategoryTypeID = (int)categoryType,
                Name = GetActivityCategoryDetailName(activityName, categoryType),
                Rate = rate,
                IsActive = true,
                CreatedBy = userId,
                CreatedDate = now,
                LastModifiedBy = lastModifiedBy ?? 0,
                LastModifiedDate = now
            });
        }
    }

    private async Task UpdateActivityCategoryDetailsAsync(ProjectActivity projectActivity, decimal skilledRate, decimal unskilledRate, decimal otherRate, int? lastModifiedBy, CancellationToken cancellationToken)
    {
        var existingDetails = await _db.Set<ActivityCategoryDetails>()
            .Where(detail => detail.ProjectID == projectActivity.ProjectID && detail.ActivityID == projectActivity.ActivityID)
            .ToListAsync(cancellationToken);
        var now = DateTime.UtcNow;
        var activityName = await GetActivityNameAsync(projectActivity.ActivityID, cancellationToken);

        foreach (var (categoryType, rate) in GetCategoryRates(skilledRate, unskilledRate, otherRate))
        {
            var detail = existingDetails.FirstOrDefault(x => x.CategoryTypeID == (int)categoryType);
            if (detail is null)
            {
                _db.Set<ActivityCategoryDetails>().Add(new ActivityCategoryDetails
                {
                    UniqueID = Guid.NewGuid(),
                    ProjectID = projectActivity.ProjectID,
                    ActivityID = projectActivity.ActivityID,
                    CategoryTypeID = (int)categoryType,
                    Name = GetActivityCategoryDetailName(activityName, categoryType),
                    Rate = rate,
                    IsActive = true,
                    CreatedBy = lastModifiedBy ?? 0,
                    CreatedDate = now,
                    LastModifiedBy = lastModifiedBy ?? 0,
                    LastModifiedDate = now
                });
                continue;
            }

            detail.Name = GetActivityCategoryDetailName(activityName, categoryType);
            detail.Rate = rate;
            detail.IsActive = true;
            detail.LastModifiedBy = lastModifiedBy ?? 0;
            detail.LastModifiedDate = now;
        }
    }

    private static IEnumerable<(CategoryType CategoryType, decimal Rate)> GetCategoryRates(decimal skilledRate,
        decimal unskilledRate, decimal otherRate)
    {
        yield return (CategoryType.Skilled, skilledRate);
        yield return (CategoryType.Unskilled, unskilledRate);
        yield return (CategoryType.Other, otherRate);
    }

    private async Task<string> GetActivityNameAsync(int activityId, CancellationToken cancellationToken)
    {
        var activityName = await _db.Set<Activity>()
            .Where(activity => activity.ID == activityId)
            .Select(activity => activity.ActivityName)
            .FirstOrDefaultAsync(cancellationToken);

        return activityName ?? throw new NotFoundException($"Activity {activityId} was not found.");
    }

    private static string GetActivityCategoryDetailName(string activityName, CategoryType categoryType) =>
        $"{activityName} - {categoryType}";

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
            cmd.CommandTimeout = 30;
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
            cmd2.CommandTimeout = 10;
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
            cmd.CommandTimeout = 30;
            cmd.Parameters.AddWithValue("@p_projectid", NpgsqlDbType.Integer, request.ProjectId);

            var da = new NpgsqlDataAdapter(cmd);
            var dt = new DataTable("Rows");
            da.Fill(dt);
            dsLocal.Tables.Add(dt);
        }

        return dsLocal;
    }

    public async Task<List<ProjectActivityCategoryDetailsModel>> Handle(GetProjectActivitiyDetailsByProjectID request, CancellationToken cancellationToken)
    {
        var result = await _db.Set<ActivityCategoryDetails>()
            .Where(x =>
                x.ProjectID == request.ProjectId &&
                x.IsActive)
            .Select(x => new ProjectActivityCategoryDetailsModel
            {
                ID = x.ID,
                ProjectID = x.ProjectID,
                Name = x.Name,
                Rate = x.Rate
            })
            .ToListAsync(cancellationToken);

        return result;
    }
}

