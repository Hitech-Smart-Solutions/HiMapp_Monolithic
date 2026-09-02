using Himapp.Workflow.Application.Features.ApprovalWorkflow.Queries;
using Himapp.Workflow.Application.Features.CentralUserRoleMapping.Commands;
using Himapp.Workflow.Application.Features.CentralUserRoleMapping.Models;
using Himapp.Workflow.Application.Features.CentralUserRoleMapping.Queries;
using Himapp.Workflow.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Data;
using CentralUserRoleMappingEntity = Himapp.Workflow.Domain.Entities.CentralUserRoleMapping;

namespace Himapp.Workflow.Application.Features.CentralUserRoleMapping.Handlers;

internal sealed class CentralUserRoleMappingHandler :
    IRequestHandler<GetAllCentralUserRoleMappingsQuery, IReadOnlyCollection<CentralUserRoleMappingDto>>,
    IRequestHandler<GetCentralUserRoleMappingByIdQuery, CentralUserRoleMappingDto?>,
    IRequestHandler<CreateCentralUserRoleMappingCommand, CentralUserRoleMappingDto>,
    IRequestHandler<UpdateCentralUserRoleMappingCommand, CentralUserRoleMappingDto?>,
    IRequestHandler<DeleteCentralUserRoleMappingCommand, bool>,
    IRequestHandler<GetRoleMappingListByCompanyQuery, DataSet>
{
    private readonly IWorkflowDbContext _db;

    public CentralUserRoleMappingHandler(IWorkflowDbContext db) => _db = db;

    public async Task<IReadOnlyCollection<CentralUserRoleMappingDto>> Handle(GetAllCentralUserRoleMappingsQuery request, CancellationToken cancellationToken)
    {
        return await _db.Set<CentralUserRoleMappingEntity>()
            .AsNoTracking()
            .Select(m => new CentralUserRoleMappingDto(
                m.ID,
                m.UniqueID,
                m.RoleCode,
                m.RoleName,
                m.StatusID,
                m.IsActive,
                m.CreatedBy,
                m.CreatedDate,
                m.LastModifiedBy,
                m.LastModifiedDate,
                Array.Empty<CentralUserRoleMappingDetailDto>()))
            .ToArrayAsync(cancellationToken);
    }

    public async Task<CentralUserRoleMappingDto?> Handle(GetCentralUserRoleMappingByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _db.Set<CentralUserRoleMappingEntity>()
            .AsNoTracking()
            .Include(x => x.Details)
            .FirstOrDefaultAsync(x => x.ID == request.Id, cancellationToken);

        return entity is null ? null : Map(entity);
    }

    public async Task<CentralUserRoleMappingDto> Handle(CreateCentralUserRoleMappingCommand command, CancellationToken cancellationToken)
    {
        var r = command.Request;

        if (string.IsNullOrWhiteSpace(r.RoleName))
            throw new InvalidOperationException("RoleName is required.");

        // Generate RoleCode automatically
        var roleCode = await GenerateNextRoleCodeAsync(cancellationToken);

        var entity = new CentralUserRoleMappingEntity
        {
            UniqueID = Guid.NewGuid(),
            RoleCode = roleCode,
            RoleName = r.RoleName,
            StatusID = r.StatusId,
            IsActive = true,
            CreatedBy = r.CreatedBy,
            CreatedDate = DateTime.UtcNow,
            LastModifiedBy = r.CreatedBy,
            LastModifiedDate = DateTime.UtcNow
        };

        if (r.Details?.Any() == true)
        {
            foreach (var d in r.Details)
            {
                entity.Details!.Add(new CentralUserRoleMappingDetails
                {
                    UniqueID = Guid.NewGuid(),
                    UserID = d.UserId,
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

        _db.Set<CentralUserRoleMappingEntity>().Add(entity);
        await _db.SaveChangesAsync(cancellationToken);

        return Map(entity);
    }

    private async Task<string> GenerateNextRoleCodeAsync(CancellationToken cancellationToken)
    {
        var lastRoleCode = await _db.Set<CentralUserRoleMappingEntity>()
            .AsNoTracking()
            .Where(x => x.RoleCode.StartsWith("ROLE-"))
            .OrderByDescending(x => x.ID)
            .Select(x => x.RoleCode)
            .FirstOrDefaultAsync(cancellationToken);

        if (string.IsNullOrWhiteSpace(lastRoleCode))
        {
            return "ROLE-000001";
        }

        var numericPart = lastRoleCode["ROLE-".Length..];

        if (!int.TryParse(numericPart, out var lastNumber))
        {
            return "ROLE-000001";
        }

        return $"ROLE-{lastNumber + 1:D6}";
    }

    public async Task<CentralUserRoleMappingDto?> Handle(UpdateCentralUserRoleMappingCommand command, CancellationToken cancellationToken)
    {
        var entity = await _db.Set<CentralUserRoleMappingEntity>()
            .Include(x => x.Details)
            .FirstOrDefaultAsync(x => x.ID == command.Id, cancellationToken);

        if (entity is null) return null;

        var r = command.Request;
        var lastModifiedBy = r.LastModifiedBy;

        if (string.IsNullOrWhiteSpace(r.RoleName))
            throw new InvalidOperationException("RoleName is required.");

        entity.RoleName = r.RoleName;
        entity.StatusID = r.StatusId;
        entity.IsActive = r.IsActive;
        entity.LastModifiedBy = lastModifiedBy;
        entity.LastModifiedDate = DateTime.UtcNow;

        if (entity.Details != null && entity.Details.Any())
        {
            _db.Set<CentralUserRoleMappingDetails>().RemoveRange(entity.Details);
            entity.Details.Clear();
        }

        if (r.Details?.Any() == true)
        {
            foreach (var d in r.Details)
            {
                entity.Details!.Add(new CentralUserRoleMappingDetails
                {
                    UniqueID = Guid.NewGuid(),
                    UserID = d.UserId,
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

        await _db.SaveChangesAsync(cancellationToken);

        return Map(entity);
    }

    public async Task<bool> Handle(DeleteCentralUserRoleMappingCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.Set<CentralUserRoleMappingEntity>()
            .Include(x => x.Details)
            .FirstOrDefaultAsync(x => x.ID == request.Id, cancellationToken);

        if (entity is null) return false;

        bool isActive = request.actionHistory.Actions == Actions.Activated;

        entity.IsActive = isActive;
        entity.LastModifiedBy = request.actionHistory.UserId;
        entity.LastModifiedDate = DateTime.UtcNow;

        foreach (var detail in entity.Details ?? [])
        {
            detail.IsActive = isActive;
            detail.LastModifiedBy = request.actionHistory.UserId;
            detail.LastModifiedDate = DateTime.UtcNow;
        }

        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static CentralUserRoleMappingDto Map(CentralUserRoleMappingEntity entity) => new(
        entity.ID,
        entity.UniqueID,
        entity.RoleCode,
        entity.RoleName,
        entity.StatusID,
        entity.IsActive,
        entity.CreatedBy,
        entity.CreatedDate,
        entity.LastModifiedBy,
        entity.LastModifiedDate,
        entity.Details?.OrderBy(x => x.ProjectID).ThenBy(x => x.UserID).Select(x => new CentralUserRoleMappingDetailDto(
            x.ID,
            x.UniqueID,
            x.UserID,
            x.ProjectID,
            x.StatusID,
            x.IsActive,
            x.CreatedBy,
            x.CreatedDate,
            x.LastModifiedBy,
            x.LastModifiedDate)).ToArray() ?? Array.Empty<CentralUserRoleMappingDetailDto>());

    public async Task<DataSet> Handle(GetRoleMappingListByCompanyQuery request, CancellationToken cancellationToken)
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
                @"SELECT * FROM public.uspgetcentralrolemappingbycompanyid(
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
                @"SELECT * FROM public.uspgetcentralrolemappingcountbycompanyid(
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
