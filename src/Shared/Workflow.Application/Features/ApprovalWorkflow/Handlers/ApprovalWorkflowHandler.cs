using Himapp.Workflow.Application.Features.ApprovalWorkflow.Commands;
using Himapp.Workflow.Application.Features.ApprovalWorkflow.Models;
using Himapp.Workflow.Application.Features.ApprovalWorkflow.Queries;
using Himapp.Workflow.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Data;

namespace Himapp.Workflow.Application.Features.ApprovalWorkflow.Handlers;

internal sealed class ApprovalWorkflowHandler :
    IRequestHandler<GetAllApprovalWorkflowsQuery, IReadOnlyCollection<ApprovalWorkflowDto>>,
    IRequestHandler<GetApprovalWorkflowByIdQuery, ApprovalWorkflowDto?>,
    IRequestHandler<CreateApprovalWorkflowCommand, ApprovalWorkflowDto>,
    IRequestHandler<UpdateApprovalWorkflowCommand, ApprovalWorkflowDto?>,
    IRequestHandler<DeleteApprovalWorkflowCommand, bool>,
    IRequestHandler<GetWorkflowListByCompanyQuery, DataSet>
{
    private readonly IWorkflowDbContext _db;

    public ApprovalWorkflowHandler(IWorkflowDbContext db) => _db = db;

    public async Task<IReadOnlyCollection<ApprovalWorkflowDto>> Handle(GetAllApprovalWorkflowsQuery request, CancellationToken cancellationToken)
    {
        return await _db.Set<CentralApprovalWorkflow>()
            .AsNoTracking()
            .Select(w => new ApprovalWorkflowDto(
                w.ID,
                w.UniqueID,
                w.ApprovalWorkflowCode,
                w.ApprovalWorkflowDate,
                w.ProgramID,
                w.CompanyID,
                w.LocationID,
                w.StatusID,
                w.IsActive,
                w.CreatedBy,
                w.CreatedDate,
                w.LastModifiedBy,
                w.LastModifiedDate,
                Array.Empty<ApprovalWorkflowProjectDetailDto>(),
                Array.Empty<ApprovalWorkflowRoleDetailDto>()))
            .ToArrayAsync(cancellationToken);
    }

    public async Task<ApprovalWorkflowDto?> Handle(GetApprovalWorkflowByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _db.Set<CentralApprovalWorkflow>()
            .AsNoTracking()
            .Include(x => x.ApprovalWorkflowProjectDetails)
            .Include(x => x.ApprovalWorkflowRoleDetails)
            .FirstOrDefaultAsync(x => x.ID == request.Id, cancellationToken);

        return entity is null ? null : Map(entity);
    }

    public async Task<ApprovalWorkflowDto> Handle(CreateApprovalWorkflowCommand command, CancellationToken cancellationToken)
    {
        var r = command.Request;

        if (string.IsNullOrWhiteSpace(r.ApprovalWorkflowCode))
            throw new InvalidOperationException("ApprovalWorkflowCode is required.");

        var entity = new CentralApprovalWorkflow
        {
            UniqueID = Guid.NewGuid(),
            ApprovalWorkflowCode = r.ApprovalWorkflowCode,
            ApprovalWorkflowDate = r.ApprovalWorkflowDate,
            ProgramID = r.ProgramId,
            CompanyID = r.CompanyId,
            LocationID = r.LocationId,
            StatusID = r.StatusId,
            IsActive = true,
            CreatedBy = r.CreatedBy,
            CreatedDate = DateTime.UtcNow,
            LastModifiedBy = r.CreatedBy,
            LastModifiedDate = DateTime.UtcNow
        };

        if (r.ProjectDetails?.Any() == true)
        {
            foreach (var d in r.ProjectDetails)
            {
                entity.ApprovalWorkflowProjectDetails!.Add(new CentralApprovalWorkflowProjectDetails
                {
                    UniqueID = Guid.NewGuid(),
                    ProjectID = d.ProjectId,
                    StatusID = d.StatusId,
                    IsActive = d.IsActive,
                    CreatedBy = r.CreatedBy,
                    CreatedDate = DateTime.UtcNow,
                    LastModifiedBy = r.CreatedBy,
                    LastModifiedDate = DateTime.UtcNow
                });
            }
        }

        if (r.RoleDetails?.Any() == true)
        {
            foreach (var d in r.RoleDetails)
            {
                entity.ApprovalWorkflowRoleDetails!.Add(new CentralApprovalWorkflowRoleDetails
                {
                    UniqueID = Guid.NewGuid(),
                    RoleID = d.RoleId,
                    Priority = d.Priority,
                    Amount = d.Amount,
                    Remarks = d.Remarks,
                    CanAuthorize = d.CanAuthorize,
                    CanUnAuthorize = d.CanUnAuthorize,
                    StatusID = d.StatusId,
                    IsActive = d.IsActive,
                    CreatedBy = r.CreatedBy,
                    CreatedDate = DateTime.UtcNow,
                    LastModifiedBy = r.CreatedBy,
                    LastModifiedDate = DateTime.UtcNow
                });
            }
        }

        _db.Set<CentralApprovalWorkflow>().Add(entity);
        await _db.SaveChangesAsync(cancellationToken);

        return Map(entity);
    }

    public async Task<ApprovalWorkflowDto?> Handle(UpdateApprovalWorkflowCommand command, CancellationToken cancellationToken)
    {
        var entity = await _db.Set<CentralApprovalWorkflow>()
            .Include(x => x.ApprovalWorkflowProjectDetails)
            .Include(x => x.ApprovalWorkflowRoleDetails)
            .FirstOrDefaultAsync(x => x.ID == command.Id, cancellationToken);

        if (entity is null) return null;

        var r = command.Request;
        var lastModifiedBy = r.LastModifiedBy;

        if (string.IsNullOrWhiteSpace(r.ApprovalWorkflowCode))
            throw new InvalidOperationException("ApprovalWorkflowCode is required.");

        entity.ApprovalWorkflowCode = r.ApprovalWorkflowCode;
        entity.ApprovalWorkflowDate = r.ApprovalWorkflowDate;
        entity.ProgramID = r.ProgramId;
        entity.CompanyID = r.CompanyId;
        entity.LocationID = r.LocationId;
        entity.StatusID = r.StatusId;
        entity.IsActive = r.IsActive;
        entity.LastModifiedBy = lastModifiedBy;
        entity.LastModifiedDate = DateTime.UtcNow;

        if (entity.ApprovalWorkflowProjectDetails != null && entity.ApprovalWorkflowProjectDetails.Any())
        {
            _db.Set<CentralApprovalWorkflowProjectDetails>().RemoveRange(entity.ApprovalWorkflowProjectDetails);
            entity.ApprovalWorkflowProjectDetails.Clear();
        }

        if (r.ProjectDetails?.Any() == true)
        {
            foreach (var d in r.ProjectDetails)
            {
                entity.ApprovalWorkflowProjectDetails!.Add(new CentralApprovalWorkflowProjectDetails
                {
                    UniqueID = Guid.NewGuid(),
                    ProjectID = d.ProjectId,
                    StatusID = d.StatusId,
                    IsActive = d.IsActive,
                    CreatedBy = lastModifiedBy,
                    CreatedDate = DateTime.UtcNow,
                    LastModifiedBy = lastModifiedBy,
                    LastModifiedDate = DateTime.UtcNow
                });
            }
        }

        if (entity.ApprovalWorkflowRoleDetails != null && entity.ApprovalWorkflowRoleDetails.Any())
        {
            _db.Set<CentralApprovalWorkflowRoleDetails>().RemoveRange(entity.ApprovalWorkflowRoleDetails);
            entity.ApprovalWorkflowRoleDetails.Clear();
        }

        if (r.RoleDetails?.Any() == true)
        {
            foreach (var d in r.RoleDetails)
            {
                entity.ApprovalWorkflowRoleDetails!.Add(new CentralApprovalWorkflowRoleDetails
                {
                    UniqueID = Guid.NewGuid(),
                    RoleID = d.RoleId,
                    Priority = d.Priority,
                    Amount = d.Amount,
                    Remarks = d.Remarks,
                    CanAuthorize = d.CanAuthorize,
                    CanUnAuthorize = d.CanUnAuthorize,
                    StatusID = d.StatusId,
                    IsActive = d.IsActive,
                    CreatedBy = lastModifiedBy,
                    CreatedDate = DateTime.UtcNow,
                    LastModifiedBy = lastModifiedBy,
                    LastModifiedDate = DateTime.UtcNow
                });
            }
        }

        await _db.SaveChangesAsync(cancellationToken);

        return Map(entity);
    }

    public async Task<bool> Handle(DeleteApprovalWorkflowCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.Set<CentralApprovalWorkflow>()
            .Include(x => x.ApprovalWorkflowProjectDetails)
            .Include(x => x.ApprovalWorkflowRoleDetails)
            .FirstOrDefaultAsync(x => x.ID == request.Id, cancellationToken);

        if (entity is null) return false;

        bool isActive = request.actionHistory.Actions == Actions.Activated;

        entity.IsActive = isActive;
        entity.LastModifiedBy = request.actionHistory.UserId;
        entity.LastModifiedDate = DateTime.UtcNow;

        foreach (var detail in entity.ApprovalWorkflowProjectDetails ?? [])
        {
            detail.IsActive = isActive;
            detail.LastModifiedBy = request.actionHistory.UserId;
            detail.LastModifiedDate = DateTime.UtcNow;
        }

        foreach (var detail in entity.ApprovalWorkflowRoleDetails ?? [])
        {
            detail.IsActive = isActive;
            detail.LastModifiedBy = request.actionHistory.UserId;
            detail.LastModifiedDate = DateTime.UtcNow;
        }

        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static ApprovalWorkflowDto Map(CentralApprovalWorkflow entity) => new(
        entity.ID,
        entity.UniqueID,
        entity.ApprovalWorkflowCode,
        entity.ApprovalWorkflowDate,
        entity.ProgramID,
        entity.CompanyID,
        entity.LocationID,
        entity.StatusID,
        entity.IsActive,
        entity.CreatedBy,
        entity.CreatedDate,
        entity.LastModifiedBy,
        entity.LastModifiedDate,
        entity.ApprovalWorkflowProjectDetails?.OrderBy(x => x.ProjectID).Select(x => new ApprovalWorkflowProjectDetailDto(
            x.ID,
            x.UniqueID,
            x.ProjectID,
            x.StatusID,
            x.IsActive,
            x.CreatedBy,
            x.CreatedDate,
            x.LastModifiedBy,
            x.LastModifiedDate)).ToArray() ?? Array.Empty<ApprovalWorkflowProjectDetailDto>(),
        entity.ApprovalWorkflowRoleDetails?.OrderBy(x => x.Priority).Select(x => new ApprovalWorkflowRoleDetailDto(
            x.ID,
            x.UniqueID,
            x.RoleID,
            x.Priority,
            x.Amount,
            x.Remarks,
            x.CanAuthorize,
            x.CanUnAuthorize,
            x.StatusID,
            x.IsActive,
            x.CreatedBy,
            x.CreatedDate,
            x.LastModifiedBy,
            x.LastModifiedDate)).ToArray() ?? Array.Empty<ApprovalWorkflowRoleDetailDto>());

    public async Task<DataSet> Handle(GetWorkflowListByCompanyQuery request, CancellationToken cancellationToken)
    {
        DataSet ds = new DataSet();

        string connectionString =
            _db.Database.GetDbConnection().ConnectionString;

        #region LIST

        using (var connection = new NpgsqlConnection(connectionString))
        {
            DataTable dt = new DataTable();

            await connection.OpenAsync(cancellationToken);

            using var cmd = new NpgsqlCommand(
                @"SELECT * FROM public.uspgetcentralapprovalworkflowbycompanyid(
                @p_companyid,
                @p_filtercolumn,
                @p_filtervalue,
                @p_pageindex,
                @p_pagesize,
                @p_sortcolumn,
                @p_isactive)",
                connection);

            cmd.Parameters.AddWithValue(
                "@p_companyid",
                request.SearchParams.CompanyID);

            cmd.Parameters.AddWithValue(
                "@p_filtercolumn",
                request.SearchParams.FilterColumn ??
                (object)DBNull.Value);

            cmd.Parameters.AddWithValue(
                "@p_filtervalue",
                request.SearchParams.FilterValue ??
                (object)DBNull.Value);

            cmd.Parameters.AddWithValue(
                "@p_pageindex",
                request.SearchParams.PageIndex);

            cmd.Parameters.AddWithValue(
                "@p_pagesize",
                request.SearchParams.PageSize);

            cmd.Parameters.AddWithValue(
                "@p_sortcolumn",
                request.SearchParams.SortColumn ??
                (object)DBNull.Value);

            cmd.Parameters.AddWithValue(
                "@p_isactive",
                request.SearchParams.IsActive ?? "true");

            using var da = new NpgsqlDataAdapter(cmd);

            da.Fill(dt);

            ds.Tables.Add(dt);
        }

        #endregion

        #region COUNT

        using (var connection = new NpgsqlConnection(connectionString))
        {
            DataTable dt = new DataTable();

            await connection.OpenAsync(cancellationToken);

            using var cmd = new NpgsqlCommand(
                @"SELECT * FROM public.uspgetcentralapprovalworkflowcountbycompanyid(
                @p_companyid,
                @p_filtercolumn,
                @p_filtervalue,
                @p_pageindex,
                @p_pagesize,
                @p_sortcolumn,
                @p_isactive)",
                connection);

            cmd.Parameters.AddWithValue(
                "@p_companyid",
                request.SearchParams.CompanyID);

            cmd.Parameters.AddWithValue(
                "@p_filtercolumn",
                request.SearchParams.FilterColumn ??
                (object)DBNull.Value);

            cmd.Parameters.AddWithValue(
                "@p_filtervalue",
                request.SearchParams.FilterValue ??
                (object)DBNull.Value);

            cmd.Parameters.AddWithValue(
                "@p_pageindex",
                request.SearchParams.PageIndex);

            cmd.Parameters.AddWithValue(
                "@p_pagesize",
                request.SearchParams.PageSize);

            cmd.Parameters.AddWithValue(
                "@p_sortcolumn",
                request.SearchParams.SortColumn ??
                (object)DBNull.Value);

            cmd.Parameters.AddWithValue(
                "@p_isactive",
                request.SearchParams.IsActive ?? "true");

            using var da = new NpgsqlDataAdapter(cmd);

            da.Fill(dt);

            ds.Tables.Add(dt);
        }

        #endregion

        return ds;
    }
}
