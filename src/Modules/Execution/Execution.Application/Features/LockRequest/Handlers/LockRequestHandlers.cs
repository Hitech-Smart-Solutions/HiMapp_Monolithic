using Himapp.Admin.Contracts.Projects;
using Himapp.Execution.Application.Features.LockRequest.Commands;
using Himapp.Execution.Application.Features.LockRequest.Models;
using Himapp.Execution.Application.Features.LockRequest.Queries;
using Himapp.Execution.Contracts;
using Himapp.Execution.Contracts.References;
using Himapp.Execution.Domain.Entities;
using Himapp.SharedKernel.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using NpgsqlTypes;
using System.Data;
using LockRequestEntity = Himapp.Execution.Domain.Entities.LockRequest;

namespace Himapp.Execution.Application.Features.LockRequest.Handlers;

internal sealed class LockRequestHandlers :
    IRequestHandler<GetAllLockRequestsQuery, IReadOnlyCollection<LockRequestModel>>,
    IRequestHandler<GetLockRequestByIdQuery, LockRequestModel?>,
    IRequestHandler<CreateLockRequestCommand, LockRequestModel>,
    IRequestHandler<UpdateLockRequestCommand, LockRequestModel?>,
    IRequestHandler<DeleteLockRequestCommand, bool>,
    IRequestHandler<DeleteLockRequestActionCommand, bool>,
    IRequestHandler<GetLockRequestByProjectIdQuery, DataSet>
{
    private readonly IExecutionDbContext _db;
    private readonly IProjectDirectory _projectDirectory;
    private readonly ILockRequestCodeGenerator _codeGenerator;
    private readonly ICurrentUser _currentUser;

    public LockRequestHandlers(IExecutionDbContext db, IProjectDirectory projectDirectory, ILockRequestCodeGenerator codeGenerator, ICurrentUser currentUser) =>
        (_db, _projectDirectory, _codeGenerator, _currentUser) = (db, projectDirectory, codeGenerator, currentUser);

    private int CurrentUserId => _currentUser.UserId ?? 5633;

    public async Task<IReadOnlyCollection<LockRequestModel>> Handle(GetAllLockRequestsQuery request, CancellationToken cancellationToken)
    {
        // Return header-only projection for performance (details omitted)
        return await _db.Set<LockRequestEntity>()
            .AsNoTracking()
            .Where(d => d.IsActive)
            .Select(d => new LockRequestModel(
                d.ID,
                d.UniqueID,
                d.RequestCode,
                d.RequestDate,
                d.Remarks,
                d.CompanyID,
                d.ProjectID,
                d.ProgramID,
                d.StateID,
                d.IsActive,
                d.CreatedBy,
                d.CreatedDate,
                d.LastModifiedBy,
                d.LastModifiedDate,
                Array.Empty<LockRequestDetailModel>()))
            .ToArrayAsync(cancellationToken);
    }

    public async Task<bool> Handle(DeleteLockRequestActionCommand request, CancellationToken cancellationToken)
    {
        var model = request.addTransactionActionHistoryDTO;
        var entity = await _db.Set<LockRequestEntity>().FirstOrDefaultAsync(a => a.ID == model.ProgramRowId, cancellationToken);

        if (entity is null) return false;

        // Mark child detail records active/inactive
        var details = await _db.Set<LockRequestDetails>()
            .Where(x => x.LockRequestID == model.ProgramRowId)
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

    public async Task<LockRequestModel?> Handle(GetLockRequestByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _db.Set<LockRequestEntity>()
            .AsNoTracking()
            .Include(d => d.LockRequestDetails)
            .FirstOrDefaultAsync(x => x.ID == request.Id, cancellationToken);

        if (entity is null) return null;

        var details = entity.LockRequestDetails?.Select(dd => new LockRequestDetailModel(
            dd.ID,
            dd.UniqueID,
            dd.OpenDate,
            dd.Reason,
            dd.Duration,
            dd.Counts,
            dd.IsActive)).ToArray()
            ?? Array.Empty<LockRequestDetailModel>();

        return new LockRequestModel(
            entity.ID,
            entity.UniqueID,
            entity.RequestCode,
            entity.RequestDate,
            entity.Remarks,
            entity.CompanyID,
            entity.ProjectID,
            entity.ProgramID,
            entity.StateID,
            entity.IsActive,
            entity.CreatedBy,
            entity.CreatedDate,
            entity.LastModifiedBy,
            entity.LastModifiedDate,
            details);
    }

    public async Task<LockRequestModel> Handle(CreateLockRequestCommand request, CancellationToken cancellationToken)
    {
        var r = request.Request;
        var userId = CurrentUserId;

        if (!Enum.IsDefined(typeof(LockRequestState), (short)r.Status))
        {
            throw new ArgumentException("Invalid Lock Request status.");
        }

        var entity = new LockRequestEntity
        {
            UniqueID = Guid.NewGuid(),
            ProjectID = r.ProjectId,
            CompanyID = r.CompanyID ?? 0,
            ProgramID = r.ProgramID,
            RequestDate = DateTime.SpecifyKind(r.RequestDate, DateTimeKind.Utc),
            Remarks = r.Remarks,
            StateID = (short)r.Status,
            IsActive = true,
            CreatedBy = userId,
            CreatedDate = DateTime.UtcNow,
            LastModifiedBy = userId,
            LastModifiedDate = DateTime.UtcNow
        };

        // Generate RequestCode using shared LockRequest code generator service
        var generatedCode = await _codeGenerator.GenerateLockRequestCodeAsync(r.ProjectId, cancellationToken);
        entity.RequestCode = string.IsNullOrWhiteSpace(generatedCode) ? null : generatedCode;

        // Add details (if any)
        if (r.Details?.Any() == true)
        {
            foreach (var d in r.Details)
            {
                var detail = new LockRequestDetails
                {
                    UniqueID = Guid.NewGuid(),
                    OpenDate = DateTime.SpecifyKind(d.OpenDate, DateTimeKind.Utc),
                    Reason = d.Reason,
                    Duration = d.Duration,
                    Counts = d.Counts,
                    IsActive = true,
                    CreatedBy = userId,
                    CreatedDate = DateTime.UtcNow,
                    LastModifiedBy = userId,
                    LastModifiedDate = DateTime.UtcNow,
                    LockRequest = entity
                };

                entity.LockRequestDetails?.Add(detail);
            }
        }

        _db.Set<LockRequestEntity>().Add(entity);
        await _db.SaveChangesAsync(cancellationToken);

        var details = entity.LockRequestDetails?.Select(dd => new LockRequestDetailModel(
            dd.ID, dd.UniqueID, dd.OpenDate, dd.Reason, dd.Duration, dd.Counts, dd.IsActive)).ToArray()
            ?? Array.Empty<LockRequestDetailModel>();

        return new LockRequestModel(
            entity.ID, entity.UniqueID, entity.RequestCode, entity.RequestDate, entity.Remarks,
            entity.CompanyID, entity.ProjectID, entity.ProgramID, entity.StateID, entity.IsActive,
            entity.CreatedBy, entity.CreatedDate, entity.LastModifiedBy, entity.LastModifiedDate, details);
    }

    public async Task<bool> Handle(DeleteLockRequestCommand request, CancellationToken cancellationToken)
    {
        var userId = CurrentUserId;
        var entity = await _db.Set<LockRequestEntity>()
            .Include(d => d.LockRequestDetails)
            .FirstOrDefaultAsync(x => x.ID == request.Id && x.IsActive, cancellationToken);
        if (entity is null) return false;

        // Soft delete header and child details
        entity.IsActive = false;
        entity.LastModifiedBy = userId;
        entity.LastModifiedDate = DateTime.UtcNow;

        if (entity.LockRequestDetails != null)
        {
            foreach (var dd in entity.LockRequestDetails)
            {
                dd.IsActive = false;
                dd.LastModifiedBy = userId;
                dd.LastModifiedDate = DateTime.UtcNow;
            }
        }

        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<LockRequestModel?> Handle(UpdateLockRequestCommand request, CancellationToken cancellationToken)
    {
        var userId = CurrentUserId;
        var entity = await _db.Set<LockRequestEntity>()
            .Include(d => d.LockRequestDetails)
            .FirstOrDefaultAsync(x => x.ID == request.Id && x.IsActive, cancellationToken);

        if (entity is null) return null;

        var r = request.Request;

        if (!Enum.IsDefined(typeof(LockRequestState), (short)r.Status))
        {
            throw new ArgumentException("Invalid Lock Request status.");
        }

        if (entity.StateID == (short)LockRequestState.Submitted)
        {
            throw new InvalidOperationException(
                "Submitted Lock Request cannot be modified.");
        }

        entity.ProjectID = r.ProjectId;
        entity.RequestDate = DateTime.SpecifyKind(r.RequestDate, DateTimeKind.Utc);
        entity.Remarks = r.Remarks;
        entity.StateID = (short)r.Status;
        entity.LastModifiedBy = userId;
        entity.LastModifiedDate = DateTime.UtcNow;

        // Remove existing details (physically) and add new ones
        if (entity.LockRequestDetails != null && entity.LockRequestDetails.Any())
        {
            _db.Set<LockRequestDetails>().RemoveRange(entity.LockRequestDetails);
            entity.LockRequestDetails.Clear();
        }

        if (r.Details?.Any() == true)
        {
            foreach (var d in r.Details)
            {
                var detail = new LockRequestDetails
                {
                    UniqueID = Guid.NewGuid(),
                    OpenDate = DateTime.SpecifyKind(d.OpenDate, DateTimeKind.Utc),
                    Reason = d.Reason,
                    Duration = d.Duration,
                    Counts = d.Counts,
                    IsActive = true,
                    CreatedBy = userId,
                    CreatedDate = DateTime.UtcNow,
                    LastModifiedBy = userId,
                    LastModifiedDate = DateTime.UtcNow,
                    LockRequest = entity
                };

                entity.LockRequestDetails?.Add(detail);
            }
        }

        await _db.SaveChangesAsync(cancellationToken);

        var details = entity.LockRequestDetails?.Select(dd => new LockRequestDetailModel(
            dd.ID, dd.UniqueID, dd.OpenDate, dd.Reason, dd.Duration, dd.Counts, dd.IsActive)).ToArray()
            ?? Array.Empty<LockRequestDetailModel>();

        return new LockRequestModel(
            entity.ID, entity.UniqueID, entity.RequestCode, entity.RequestDate, entity.Remarks,
            entity.CompanyID, entity.ProjectID, entity.ProgramID, entity.StateID, entity.IsActive,
            entity.CreatedBy, entity.CreatedDate, entity.LastModifiedBy, entity.LastModifiedDate, details);
    }

    public async Task<DataSet> Handle(GetLockRequestByProjectIdQuery request, CancellationToken cancellationToken)
    {
        var p = request.SearchParamsProjectWise ?? new SearchParamsProjectWise();

        var dbContext = _db as DbContext;
        if (dbContext is null)
            throw new InvalidOperationException("IExecutionDbContext is not a DbContext. Cannot obtain connection string for Npgsql operations.");

        var dsLocal = new DataSet("LockRequestsResult");
        var connString = dbContext.Database.GetDbConnection().ConnectionString;

        using var conn = new NpgsqlConnection(connString);
        await conn.OpenAsync(cancellationToken);

        // Rows table
        using (var cmd = new NpgsqlCommand("SELECT * FROM execution.uspgetlockrequestbyprojectid(@p_projectid,@p_filtercolumn,@p_filtervalue,@p_pageindex,@p_pagesize,@p_sortcolumn,@p_isactive)", conn))
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
        using (var cmd2 = new NpgsqlCommand("SELECT cnt FROM execution.uspgetlockrequestcountbyprojectid(@p_projectid,@p_filtercolumn,@p_filtervalue,@p_pageindex,@p_pagesize,@p_sortcolumn,@p_isactive)", conn))
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
}
