using Himapp.PM.Application.Features.EquipmentBreakdownReasons.Commands;
using Himapp.PM.Application.Features.EquipmentBreakdownReasons.Models;
using Himapp.PM.Application.Features.EquipmentBreakdownReasons.Queries;
using Himapp.PM.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using BreakdownReasonsEntity = Himapp.PM.Domain.Entities.EquipmentBreakdownReasons;

namespace Himapp.PM.Application.Features.EquipmentBreakdownReasons.Handlers;

internal sealed class CreateEquipmentBreakdownReasonsCommandHandler : IRequestHandler<CreateEquipmentBreakdownReasonsCommand, EquipmentBreakdownReasonsDto>
{
    private readonly IPMDbContext _db;

    public CreateEquipmentBreakdownReasonsCommandHandler(IPMDbContext db) => _db = db;

    public async Task<EquipmentBreakdownReasonsDto> Handle(CreateEquipmentBreakdownReasonsCommand request, CancellationToken cancellationToken)
    {
        var exists = await _db.Set<BreakdownReasonsEntity>().AsNoTracking()
            .AnyAsync(p => p.BreakdownReason.ToLower() == request.BreakdownReason.ToLower(), cancellationToken);
        if (exists)
            throw new InvalidOperationException($"An equipment breakdown reason with description '{request.BreakdownReason}' already exists.");

        var entity = new BreakdownReasonsEntity
        {
            UniqueID = Guid.NewGuid(),
            BreakdownReason = request.BreakdownReason,
            IsActive = request.IsActive,
            Type = (int)request.Type,
            CreatedBy = request.CreateBy,
            CreatedDate = DateTime.UtcNow,
            LastModifiedBy = request.LastModifiedBy,
            LastModifiedDate = DateTime.UtcNow
        };

        _db.Set<BreakdownReasonsEntity>().Add(entity);
        await _db.SaveChangesAsync(cancellationToken);

        return entity.ToDto();
    }
}

internal sealed class UpdateEquipmentBreakdownReasonsCommandHandler : IRequestHandler<UpdateEquipmentBreakdownReasonsCommand, EquipmentBreakdownReasonsDto?>
{
    private readonly IPMDbContext _db;

    public UpdateEquipmentBreakdownReasonsCommandHandler(IPMDbContext db) => _db = db;

    public async Task<EquipmentBreakdownReasonsDto?> Handle(UpdateEquipmentBreakdownReasonsCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.Set<BreakdownReasonsEntity>()
            .FirstOrDefaultAsync(p => p.ID == request.Id, cancellationToken);
        if (entity is null) return null;

        var duplicate = await _db.Set<BreakdownReasonsEntity>().AsNoTracking()
            .Where(p => p.ID != request.Id)
            .AnyAsync(p => p.BreakdownReason.ToLower() == request.BreakdownReason.ToLower(), cancellationToken);
        if (duplicate)
            throw new InvalidOperationException($"An equipment breakdown reason with description '{request.BreakdownReason}' already exists.");

        entity.BreakdownReason = request.BreakdownReason;
        entity.IsActive = request.IsActive;
        entity.Type = (int)request.Type;
        entity.LastModifiedBy = request.LastModifiedBy;
        entity.LastModifiedDate = DateTime.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);
        return entity.ToDto();
    }
}

internal sealed class DeleteEquipmentBreakdownReasonsCommandHandler : IRequestHandler<DeleteEquipmentBreakdownReasonsCommand, bool>
{
    private readonly IPMDbContext _db;

    public DeleteEquipmentBreakdownReasonsCommandHandler(IPMDbContext db) => _db = db;

    public async Task<bool> Handle(DeleteEquipmentBreakdownReasonsCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.Set<BreakdownReasonsEntity>()
            .FirstOrDefaultAsync(p => p.ID == request.Id, cancellationToken);
        if (entity is null) return false;

        entity.IsActive = false;
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }
}

internal sealed class GetAllEquipmentBreakdownReasonsQueryHandler : IRequestHandler<GetAllEquipmentBreakdownReasonsQuery, IReadOnlyCollection<EquipmentBreakdownReasonsDto>>
{
    private readonly IPMDbContext _db;

    public GetAllEquipmentBreakdownReasonsQueryHandler(IPMDbContext db) => _db = db;

    public async Task<IReadOnlyCollection<EquipmentBreakdownReasonsDto>> Handle(GetAllEquipmentBreakdownReasonsQuery request, CancellationToken cancellationToken) =>
        await _db.Set<BreakdownReasonsEntity>().AsNoTracking()
            .Where(p => p.IsActive)
            .Select(p => new EquipmentBreakdownReasonsDto(
                p.ID,
                p.BreakdownReason,
                p.IsActive,
                (EquipmentBreakdownReasonType)p.Type))
            .ToArrayAsync(cancellationToken);
}

internal sealed class GetEquipmentBreakdownReasonsByIdQueryHandler : IRequestHandler<GetEquipmentBreakdownReasonsByIdQuery, EquipmentBreakdownReasonsDto?>
{
    private readonly IPMDbContext _db;

    public GetEquipmentBreakdownReasonsByIdQueryHandler(IPMDbContext db) => _db = db;

    public async Task<EquipmentBreakdownReasonsDto?> Handle(GetEquipmentBreakdownReasonsByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _db.Set<BreakdownReasonsEntity>().AsNoTracking()
            .FirstOrDefaultAsync(p => p.ID == request.Id && p.IsActive, cancellationToken);
        return entity?.ToDto();
    }
}

file static class EquipmentBreakdownReasonsMappings
{
    public static EquipmentBreakdownReasonsDto ToDto(this BreakdownReasonsEntity entity) =>
        new(entity.ID, entity.BreakdownReason, entity.IsActive, (EquipmentBreakdownReasonType)entity.Type);
}
