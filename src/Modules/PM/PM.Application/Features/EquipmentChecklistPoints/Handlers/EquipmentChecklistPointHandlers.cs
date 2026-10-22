using Himapp.PM.Application.Features.EquipmentChecklistPoints.Commands;
using Himapp.PM.Application.Features.EquipmentChecklistPoints.Models;
using Himapp.PM.Application.Features.EquipmentChecklistPoints.Queries;
using Himapp.PM.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Himapp.PM.Application.Features.EquipmentChecklistPoints.Handlers;

internal sealed class CreateEquipmentChecklistPointCommandHandler : IRequestHandler<CreateEquipmentChecklistPointCommand, EquipmentChecklistPointDto>
{
    private readonly IPMDbContext _db;

    public CreateEquipmentChecklistPointCommandHandler(IPMDbContext db) => _db = db;

    public async Task<EquipmentChecklistPointDto> Handle(CreateEquipmentChecklistPointCommand request, CancellationToken cancellationToken)
    {
        var exists = await _db.Set<EquipmentChecklistPoint>().AsNoTracking()
            .AnyAsync(p => p.Description.ToLower() == request.Description.ToLower(), cancellationToken);
        if (exists)
            throw new InvalidOperationException($"An equipment checklist point with description '{request.Description}' already exists.");

        var entity = new EquipmentChecklistPoint
        {
            UniqueID = Guid.NewGuid(),
            Description = request.Description,
            IsActive = request.IsActive,
            Type = (int)request.Type,
            CreatedBy = request.CreateBy,
            CreatedDate = DateTime.UtcNow,
            LastModifiedBy = request.LastModifiedBy,
            LastModifiedDate = DateTime.UtcNow
        };

        _db.Set<EquipmentChecklistPoint>().Add(entity);
        await _db.SaveChangesAsync(cancellationToken);

        return entity.ToDto();
    }
}

internal sealed class UpdateEquipmentChecklistPointCommandHandler : IRequestHandler<UpdateEquipmentChecklistPointCommand, EquipmentChecklistPointDto?>
{
    private readonly IPMDbContext _db;

    public UpdateEquipmentChecklistPointCommandHandler(IPMDbContext db) => _db = db;

    public async Task<EquipmentChecklistPointDto?> Handle(UpdateEquipmentChecklistPointCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.Set<EquipmentChecklistPoint>()
            .FirstOrDefaultAsync(p => p.ID == request.Id, cancellationToken);
        if (entity is null) return null;

        var duplicate = await _db.Set<EquipmentChecklistPoint>().AsNoTracking()
            .Where(p => p.ID != request.Id)
            .AnyAsync(p => p.Description.ToLower() == request.Description.ToLower(), cancellationToken);
        if (duplicate)
            throw new InvalidOperationException($"An equipment checklist point with description '{request.Description}' already exists.");

        entity.Description = request.Description;
        entity.IsActive = request.IsActive;
        entity.Type = (int)request.Type;
        entity.LastModifiedBy = request.LastModifiedBy;
        entity.LastModifiedDate = DateTime.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);
        return entity.ToDto();
    }
}

internal sealed class DeleteEquipmentChecklistPointCommandHandler : IRequestHandler<DeleteEquipmentChecklistPointCommand, bool>
{
    private readonly IPMDbContext _db;

    public DeleteEquipmentChecklistPointCommandHandler(IPMDbContext db) => _db = db;

    public async Task<bool> Handle(DeleteEquipmentChecklistPointCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.Set<EquipmentChecklistPoint>()
            .FirstOrDefaultAsync(p => p.ID == request.Id, cancellationToken);
        if (entity is null) return false;

        entity.IsActive = false;
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }
}

internal sealed class GetAllEquipmentChecklistPointsQueryHandler : IRequestHandler<GetAllEquipmentChecklistPointsQuery, IReadOnlyCollection<EquipmentChecklistPointDto>>
{
    private readonly IPMDbContext _db;

    public GetAllEquipmentChecklistPointsQueryHandler(IPMDbContext db) => _db = db;

    public async Task<IReadOnlyCollection<EquipmentChecklistPointDto>> Handle(GetAllEquipmentChecklistPointsQuery request, CancellationToken cancellationToken) =>
        await _db.Set<EquipmentChecklistPoint>().AsNoTracking()
            .Where(p => p.IsActive)
            .Select(p => new EquipmentChecklistPointDto(
                p.ID,
                p.Description,
                p.IsActive,
                (EquipmentChecklistPointType)p.Type))
            .ToArrayAsync(cancellationToken);
}

internal sealed class GetEquipmentChecklistPointByIdQueryHandler : IRequestHandler<GetEquipmentChecklistPointByIdQuery, EquipmentChecklistPointDto?>
{
    private readonly IPMDbContext _db;

    public GetEquipmentChecklistPointByIdQueryHandler(IPMDbContext db) => _db = db;

    public async Task<EquipmentChecklistPointDto?> Handle(GetEquipmentChecklistPointByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _db.Set<EquipmentChecklistPoint>().AsNoTracking()
            .FirstOrDefaultAsync(p => p.ID == request.Id && p.IsActive, cancellationToken);
        return entity?.ToDto();
    }
}

file static class EquipmentChecklistPointMappings
{
    public static EquipmentChecklistPointDto ToDto(this EquipmentChecklistPoint entity) =>
        new(entity.ID, entity.Description, entity.IsActive, (EquipmentChecklistPointType)entity.Type);
}
