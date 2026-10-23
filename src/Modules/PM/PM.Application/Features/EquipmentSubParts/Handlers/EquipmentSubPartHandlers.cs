using Himapp.PM.Application.Features.EquipmentSubParts.Commands;
using Himapp.PM.Application.Features.Common;
using Himapp.PM.Application.Features.EquipmentSubParts.Models;
using Himapp.PM.Application.Features.EquipmentSubParts.Queries;
using Himapp.PM.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Himapp.PM.Application.Features.EquipmentSubParts.Handlers;

internal sealed class CreateEquipmentSubPartCommandHandler : IRequestHandler<CreateEquipmentSubPartCommand, EquipmentSubPartDto>
{
    private readonly IPMDbContext _db;

    public CreateEquipmentSubPartCommandHandler(IPMDbContext db) => _db = db;

    public async Task<EquipmentSubPartDto> Handle(CreateEquipmentSubPartCommand request, CancellationToken cancellationToken)
    {
        var exists = await _db.Set<EquipmentSubPart>().AsNoTracking()
            .AnyAsync(d => d.EquipmentSubPartName.ToLower() == request.EquipmentSubPartName.ToLower(), cancellationToken);
        if (exists)
            throw new InvalidOperationException($"An equipment sub-part with name '{request.EquipmentSubPartName}' already exists.");

        var entity = new EquipmentSubPart
        {
            UniqueID = Guid.NewGuid(),
            EquipmentSubPartName = request.EquipmentSubPartName,
            IsActive = request.IsActive,
            CreatedBy = request.CreateBy,
            CreatedDate = DateTime.UtcNow,
            LastModifiedBy = request.LastModifiedBy,
            LastModifiedDate = DateTime.UtcNow
        };

        _db.Set<EquipmentSubPart>().Add(entity);
        await _db.SaveChangesAsync(cancellationToken);

        return new EquipmentSubPartDto(entity.ID, entity.EquipmentSubPartName, entity.IsActive);
    }
}

internal sealed class UpdateEquipmentSubPartCommandHandler : IRequestHandler<UpdateEquipmentSubPartCommand, EquipmentSubPartDto?>
{
    private readonly IPMDbContext _db;

    public UpdateEquipmentSubPartCommandHandler(IPMDbContext db) => _db = db;

    public async Task<EquipmentSubPartDto?> Handle(UpdateEquipmentSubPartCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.Set<EquipmentSubPart>().FirstOrDefaultAsync(d => d.ID == request.Id, cancellationToken);
        if (entity is null) return null;

        var isDuplicate = await _db.Set<EquipmentSubPart>().AsNoTracking()
            .Where(d => d.ID != request.Id)
            .AnyAsync(d => d.EquipmentSubPartName.ToLower() == request.EquipmentSubPartName.ToLower(), cancellationToken);
        if (isDuplicate)
            throw new InvalidOperationException($"An equipment sub-part with name '{request.EquipmentSubPartName}' already exists.");

        entity.EquipmentSubPartName = request.EquipmentSubPartName;
        entity.IsActive = request.IsActive;
        entity.LastModifiedBy = request.LastModifiedBy;
        entity.LastModifiedDate = DateTime.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);
        return new EquipmentSubPartDto(entity.ID, entity.EquipmentSubPartName, entity.IsActive);
    }
}

internal sealed class DeleteEquipmentSubPartCommandHandler : IRequestHandler<DeleteEquipmentSubPartCommand, bool>
{
    private readonly IPMDbContext _db;

    public DeleteEquipmentSubPartCommandHandler(IPMDbContext db) => _db = db;

    public async Task<bool> Handle(DeleteEquipmentSubPartCommand request, CancellationToken cancellationToken)
    {
        var actionHistory = request.addTransactionActionHistoryDTO;
        var entity = await _db.Set<EquipmentSubPart>()
            .FirstOrDefaultAsync(d => d.ID == actionHistory.ProgramRowId, cancellationToken);
        if (entity is null) return false;

        entity.IsActive = actionHistory.Actions == Actions.Activated;
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }
}

internal sealed class GetAllEquipmentSubPartsQueryHandler : IRequestHandler<GetAllEquipmentSubPartsQuery, IReadOnlyCollection<EquipmentSubPartDto>>
{
    private readonly IPMDbContext _db;

    public GetAllEquipmentSubPartsQueryHandler(IPMDbContext db) => _db = db;

    public async Task<IReadOnlyCollection<EquipmentSubPartDto>> Handle(GetAllEquipmentSubPartsQuery request, CancellationToken cancellationToken)
    {
        return await _db.Set<EquipmentSubPart>().AsNoTracking()
            .Where(d => d.IsActive)
            .Select(d => new EquipmentSubPartDto(d.ID, d.EquipmentSubPartName, d.IsActive))
            .ToArrayAsync(cancellationToken);
    }
}

internal sealed class GetEquipmentSubPartByIdQueryHandler : IRequestHandler<GetEquipmentSubPartByIdQuery, EquipmentSubPartDto?>
{
    private readonly IPMDbContext _db;

    public GetEquipmentSubPartByIdQueryHandler(IPMDbContext db) => _db = db;

    public async Task<EquipmentSubPartDto?> Handle(GetEquipmentSubPartByIdQuery request, CancellationToken cancellationToken)
    {
        var dto = await _db.Set<EquipmentSubPart>().AsNoTracking()
            .FirstOrDefaultAsync(d => d.ID == request.Id && d.IsActive, cancellationToken);
        return dto is null ? null : new EquipmentSubPartDto(dto.ID, dto.EquipmentSubPartName, dto.IsActive);
    }
}

internal sealed class GetEquipmentSubPartListHandler : IRequestHandler<GetEquipmentSubPartList, PagedResult<EquipmentSubPartListItemDto>>
{
    private readonly IPMDbContext _db;

    public GetEquipmentSubPartListHandler(IPMDbContext db) => _db = db;

    public Task<PagedResult<EquipmentSubPartListItemDto>> Handle(GetEquipmentSubPartList request, CancellationToken cancellationToken) =>
        PagedStoredProcedureQueryExecutor.ExecuteAsync(
            _db,
            "uspgetequipmentsubpart",
            "uspgetequipmentsubpartcount",
            request.Page,
            request.PageSize,
            request.FilterValue,
            request.SortColumn,
            request.IsActive,
            reader => new EquipmentSubPartListItemDto(
                reader.GetInt32(reader.GetOrdinal("ID")),
                reader.GetString(reader.GetOrdinal("EquipmentSubPartName")),
                reader.GetBoolean(reader.GetOrdinal("IsActive")),
                reader.GetGuid(reader.GetOrdinal("UniqueID")),
                reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
                reader.GetInt32(reader.GetOrdinal("CreatedBy")),
                reader.GetDateTime(reader.GetOrdinal("LastModifiedDate")),
                reader.GetInt32(reader.GetOrdinal("LastModifiedBy"))),
            cancellationToken);
}
