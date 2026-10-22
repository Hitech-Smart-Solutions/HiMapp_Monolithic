using Himapp.PM.Application.Features.Common;
using Himapp.PM.Application.Features.DynamicsEquipmentMasters.Commands;
using Himapp.PM.Application.Features.DynamicsEquipmentMasters.Models;
using Himapp.PM.Application.Features.DynamicsEquipmentMasters.Queries;
using Himapp.PM.Domain.Entities;
using Himapp.SharedKernel.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Himapp.PM.Application.Features.DynamicsEquipmentMasters.Handlers;

internal sealed class CreateDynamicsEquipmentMasterCommandHandler : IRequestHandler<CreateDynamicsEquipmentMasterCommand, DynamicsEquipmentMasterDto>
{
    private readonly IPMDbContext _db;
    private readonly ICurrentUser _currentUser;
    public CreateDynamicsEquipmentMasterCommandHandler(IPMDbContext db, ICurrentUser currentUser) => (_db, _currentUser) = (db, currentUser);

    private int CurrentUserId => _currentUser.UserId ?? throw new UnauthorizedAccessException("An authenticated user is required.");

    public async Task<DynamicsEquipmentMasterDto> Handle(CreateDynamicsEquipmentMasterCommand command, CancellationToken cancellationToken)
    {
        var userId = CurrentUserId;
        var r = command.Request;
        var exists = await _db.Set<DynamicsEquipmentMaster>().AsNoTracking()
            .AnyAsync(m => m.DataAreaId == r.DataAreaId && m.OBJECTID == r.OBJECTID, cancellationToken);
        if (exists)
            throw new InvalidOperationException($"Equipment with DataAreaId '{r.DataAreaId}' and OBJECTID '{r.OBJECTID}' already exists.");

        var now = DateTime.UtcNow;
        var entity = (DynamicsEquipmentMaster)Activator.CreateInstance(typeof(DynamicsEquipmentMaster), nonPublic: true)!;
        entity.UniqueID = Guid.NewGuid();
        entity.CreatedBy = userId;
        entity.CreatedDate = now;
        entity.LastModifiedBy = userId;
        entity.LastModifiedDate = now;
        DynamicsEquipmentMasterMapper.Apply(entity, r);

        _db.Set<DynamicsEquipmentMaster>().Add(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return DynamicsEquipmentMasterMapper.ToDto(entity);
    }
}

internal sealed class UpdateDynamicsEquipmentMasterCommandHandler : IRequestHandler<UpdateDynamicsEquipmentMasterCommand, DynamicsEquipmentMasterDto?>
{
    private readonly IPMDbContext _db;
    private readonly ICurrentUser _currentUser;
    public UpdateDynamicsEquipmentMasterCommandHandler(IPMDbContext db, ICurrentUser currentUser) => (_db, _currentUser) = (db, currentUser);

    private int CurrentUserId => _currentUser.UserId ?? throw new UnauthorizedAccessException("An authenticated user is required.");

    public async Task<DynamicsEquipmentMasterDto?> Handle(UpdateDynamicsEquipmentMasterCommand command, CancellationToken cancellationToken)
    {
        var userId = CurrentUserId;
        var r = command.Request;
        var entity = await _db.Set<DynamicsEquipmentMaster>().FirstOrDefaultAsync(m => m.ID == command.Id, cancellationToken);
        if (entity is null) return null;

        var duplicate = await _db.Set<DynamicsEquipmentMaster>().AsNoTracking()
            .AnyAsync(m => m.ID != command.Id && m.DataAreaId == r.DataAreaId && m.OBJECTID == r.OBJECTID, cancellationToken);
        if (duplicate)
            throw new InvalidOperationException($"Equipment with DataAreaId '{r.DataAreaId}' and OBJECTID '{r.OBJECTID}' already exists.");

        DynamicsEquipmentMasterMapper.Apply(entity, r);
        entity.LastModifiedBy = userId;
        entity.LastModifiedDate = DateTime.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);
        return DynamicsEquipmentMasterMapper.ToDto(entity);
    }
}

internal sealed class DeleteDynamicsEquipmentMasterCommandHandler : IRequestHandler<DeleteDynamicsEquipmentMasterCommand, bool>
{
    private readonly IPMDbContext _db;
    private readonly ICurrentUser _currentUser;
    public DeleteDynamicsEquipmentMasterCommandHandler(IPMDbContext db, ICurrentUser currentUser) => (_db, _currentUser) = (db, currentUser);

    private int CurrentUserId => _currentUser.UserId ?? throw new UnauthorizedAccessException("An authenticated user is required.");

    public async Task<bool> Handle(DeleteDynamicsEquipmentMasterCommand command, CancellationToken cancellationToken)
    {
        var userId = CurrentUserId;
        var entity = await _db.Set<DynamicsEquipmentMaster>().FirstOrDefaultAsync(m => m.ID == command.Id, cancellationToken);
        if (entity is null) return false;

        // TODO: soft delete (IsActive = false) once the IsActive column is migrated. For now this only stamps the audit fields.
        entity.LastModifiedBy = userId;
        entity.LastModifiedDate = DateTime.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }
}

internal sealed class GetDynamicsEquipmentMastersQueryHandler : IRequestHandler<GetDynamicsEquipmentMastersQuery, PagedResult<DynamicsEquipmentMasterDto>>
{
    private readonly IPMDbContext _db;
    public GetDynamicsEquipmentMastersQueryHandler(IPMDbContext db) => _db = db;

    public async Task<PagedResult<DynamicsEquipmentMasterDto>> Handle(GetDynamicsEquipmentMastersQuery query, CancellationToken cancellationToken)
    {
        var p = query.Params;
        var page = Math.Max(1, p.PageIndex);
        var pageSize = Math.Clamp(p.PageSize, 1, 200);
        var filterColumn = string.IsNullOrWhiteSpace(p.FilterColumn) ? null : p.FilterColumn.Trim();
        var filterValue = string.IsNullOrWhiteSpace(p.FilterValue) ? null : p.FilterValue.Trim();
        var sortColumn = string.IsNullOrWhiteSpace(p.SortColumn) ? null : p.SortColumn.Trim();
        var sortDirection = string.Equals(p.SortDirection?.Trim(), "desc", StringComparison.OrdinalIgnoreCase) ? "desc" : "asc";

        try
        {
            // Paging, filtering and sorting are done by the PostgreSQL functions created in migration AddEquipmentListFunctions.
            var rows = await _db.Set<DynamicsEquipmentMaster>()
                .FromSqlInterpolated($"SELECT * FROM \"PAndM\".uspgetdynamicsequipmentmasters({filterColumn}::text, {filterValue}::text, {page}, {pageSize}, {sortColumn}::text, {sortDirection}::text)")
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            var total = await _db.Database
                .SqlQuery<long>($"SELECT cnt AS \"Value\" FROM \"PAndM\".uspgetdynamicsequipmentmasterscount({filterColumn}::text, {filterValue}::text)")
                .SingleAsync(cancellationToken);

            return new PagedResult<DynamicsEquipmentMasterDto>(rows.Select(DynamicsEquipmentMasterMapper.ToDto).ToArray(), page, pageSize, (int)total);
        }
        catch (Npgsql.PostgresException ex) when (ex.SqlState == "22023")
        {
            // Raised by the function when the filter column does not exist.
            throw new ArgumentException(ex.MessageText);
        }
    }
}

internal sealed class GetDynamicsEquipmentMasterByIdQueryHandler : IRequestHandler<GetDynamicsEquipmentMasterByIdQuery, DynamicsEquipmentMasterDto?>
{
    private readonly IPMDbContext _db;
    public GetDynamicsEquipmentMasterByIdQueryHandler(IPMDbContext db) => _db = db;

    public async Task<DynamicsEquipmentMasterDto?> Handle(GetDynamicsEquipmentMasterByIdQuery query, CancellationToken cancellationToken)
    {
        var entity = await _db.Set<DynamicsEquipmentMaster>().AsNoTracking()
            .FirstOrDefaultAsync(m => m.ID == query.Id, cancellationToken);
        return entity is null ? null : DynamicsEquipmentMasterMapper.ToDto(entity);
    }
}
