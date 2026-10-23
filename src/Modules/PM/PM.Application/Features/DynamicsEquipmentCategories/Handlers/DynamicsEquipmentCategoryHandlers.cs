using Himapp.PM.Application.Features.Common;
using Himapp.PM.Application.Features.DynamicsEquipmentCategories.Commands;
using Himapp.PM.Application.Features.DynamicsEquipmentCategories.Models;
using Himapp.PM.Application.Features.DynamicsEquipmentCategories.Queries;
using Himapp.PM.Domain.Entities;
using Himapp.SharedKernel.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Himapp.PM.Application.Features.DynamicsEquipmentCategories.Handlers;

internal sealed class CreateDynamicsEquipmentCategoryCommandHandler : IRequestHandler<CreateDynamicsEquipmentCategoryCommand, DynamicsEquipmentCategoryDto>
{
    private readonly IPMDbContext _db;
    private readonly ICurrentUser _currentUser;
    public CreateDynamicsEquipmentCategoryCommandHandler(IPMDbContext db, ICurrentUser currentUser) => (_db, _currentUser) = (db, currentUser);

    private int CurrentUserId => _currentUser.UserId ?? throw new UnauthorizedAccessException("An authenticated user is required.");

    public async Task<DynamicsEquipmentCategoryDto> Handle(CreateDynamicsEquipmentCategoryCommand command, CancellationToken cancellationToken)
    {
        var userId = CurrentUserId;
        var r = command.Request;
        var exists = await _db.Set<DynamicsEquipmentCategory>().AsNoTracking()
            .AnyAsync(c => c.DataAreaId == r.DataAreaId && c.OBJECTTYPEID == r.OBJECTTYPEID, cancellationToken);
        if (exists)
            throw new InvalidOperationException($"An equipment category with DataAreaId '{r.DataAreaId}' and OBJECTTYPEID '{r.OBJECTTYPEID}' already exists.");

        var now = DateTime.UtcNow;
        var entity = new DynamicsEquipmentCategory
        {
            UniqueID = Guid.NewGuid(),
            CreatedBy = userId,
            CreatedDate = now,
            LastModifiedBy = userId,
            LastModifiedDate = now
        };
        DynamicsEquipmentCategoryMapper.Apply(entity, r);

        _db.Set<DynamicsEquipmentCategory>().Add(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return DynamicsEquipmentCategoryMapper.ToDto(entity);
    }
}

internal sealed class UpdateDynamicsEquipmentCategoryCommandHandler : IRequestHandler<UpdateDynamicsEquipmentCategoryCommand, DynamicsEquipmentCategoryDto?>
{
    private readonly IPMDbContext _db;
    private readonly ICurrentUser _currentUser;
    public UpdateDynamicsEquipmentCategoryCommandHandler(IPMDbContext db, ICurrentUser currentUser) => (_db, _currentUser) = (db, currentUser);

    private int CurrentUserId => _currentUser.UserId ?? throw new UnauthorizedAccessException("An authenticated user is required.");

    public async Task<DynamicsEquipmentCategoryDto?> Handle(UpdateDynamicsEquipmentCategoryCommand command, CancellationToken cancellationToken)
    {
        var userId = CurrentUserId;
        var r = command.Request;
        var entity = await _db.Set<DynamicsEquipmentCategory>().FirstOrDefaultAsync(c => c.ID == command.Id, cancellationToken);
        if (entity is null) return null;

        var duplicate = await _db.Set<DynamicsEquipmentCategory>().AsNoTracking()
            .AnyAsync(c => c.ID != command.Id && c.DataAreaId == r.DataAreaId && c.OBJECTTYPEID == r.OBJECTTYPEID, cancellationToken);
        if (duplicate)
            throw new InvalidOperationException($"An equipment category with DataAreaId '{r.DataAreaId}' and OBJECTTYPEID '{r.OBJECTTYPEID}' already exists.");

        DynamicsEquipmentCategoryMapper.Apply(entity, r);
        entity.LastModifiedBy = userId;
        entity.LastModifiedDate = DateTime.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);
        return DynamicsEquipmentCategoryMapper.ToDto(entity);
    }
}

internal sealed class DeleteDynamicsEquipmentCategoryCommandHandler : IRequestHandler<DeleteDynamicsEquipmentCategoryCommand, bool>
{
    private readonly IPMDbContext _db;
    private readonly ICurrentUser _currentUser;
    public DeleteDynamicsEquipmentCategoryCommandHandler(IPMDbContext db, ICurrentUser currentUser) => (_db, _currentUser) = (db, currentUser);

    private int CurrentUserId => _currentUser.UserId ?? throw new UnauthorizedAccessException("An authenticated user is required.");

    public async Task<bool> Handle(DeleteDynamicsEquipmentCategoryCommand command, CancellationToken cancellationToken)
    {
        var userId = CurrentUserId;
        var entity = await _db.Set<DynamicsEquipmentCategory>().FirstOrDefaultAsync(c => c.ID == command.Id, cancellationToken);
        if (entity is null) return false;
        entity.IsActive = false; // soft delete
        entity.LastModifiedBy = userId;
        entity.LastModifiedDate = DateTime.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }
}

internal sealed class GetDynamicsEquipmentCategoriesQueryHandler : IRequestHandler<GetDynamicsEquipmentCategoriesQuery, PagedResult<DynamicsEquipmentCategoryDto>>
{
    private readonly IPMDbContext _db;
    public GetDynamicsEquipmentCategoriesQueryHandler(IPMDbContext db) => _db = db;

    public async Task<PagedResult<DynamicsEquipmentCategoryDto>> Handle(GetDynamicsEquipmentCategoriesQuery query, CancellationToken cancellationToken)
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
            var rows = await _db.Set<DynamicsEquipmentCategory>()
                .FromSqlInterpolated($"SELECT * FROM \"PAndM\".uspgetdynamicsequipmentcategories({filterColumn}::text, {filterValue}::text, {page}, {pageSize}, {sortColumn}::text, {sortDirection}::text)")
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            var total = await _db.Database
                .SqlQuery<long>($"SELECT cnt AS \"Value\" FROM \"PAndM\".uspgetdynamicsequipmentcategoriescount({filterColumn}::text, {filterValue}::text)")
                .SingleAsync(cancellationToken);

            return new PagedResult<DynamicsEquipmentCategoryDto>(rows.Select(DynamicsEquipmentCategoryMapper.ToDto).ToArray(), page, pageSize, (int)total);
        }
        catch (Npgsql.PostgresException ex) when (ex.SqlState == "22023")
        {
            // Raised by the function when the filter column does not exist.
            throw new ArgumentException(ex.MessageText);
        }
    }
}

internal sealed class GetDynamicsEquipmentCategoryByIdQueryHandler : IRequestHandler<GetDynamicsEquipmentCategoryByIdQuery, DynamicsEquipmentCategoryDto?>
{
    private readonly IPMDbContext _db;
    public GetDynamicsEquipmentCategoryByIdQueryHandler(IPMDbContext db) => _db = db;

    public async Task<DynamicsEquipmentCategoryDto?> Handle(GetDynamicsEquipmentCategoryByIdQuery query, CancellationToken cancellationToken)
    {
        var entity = await _db.Set<DynamicsEquipmentCategory>().AsNoTracking()
            .FirstOrDefaultAsync(c => c.ID == query.Id, cancellationToken);
        return entity is null ? null : DynamicsEquipmentCategoryMapper.ToDto(entity);
    }
}
