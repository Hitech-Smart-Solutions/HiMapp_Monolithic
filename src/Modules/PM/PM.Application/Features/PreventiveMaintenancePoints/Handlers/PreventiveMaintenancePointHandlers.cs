using Himapp.PM.Application.Features.PreventiveMaintenancePoints.Commands;
using Himapp.PM.Application.Features.PreventiveMaintenancePoints.Models;
using Himapp.PM.Application.Features.PreventiveMaintenancePoints.Queries;
using Himapp.PM.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Himapp.PM.Application.Features.PreventiveMaintenancePoints.Handlers;

internal sealed class CreatePreventiveMaintenancePointCommandHandler : IRequestHandler<CreatePreventiveMaintenancePointCommand, PreventiveMaintenancePointDto>
{
    private readonly IPMDbContext _db;

    public CreatePreventiveMaintenancePointCommandHandler(IPMDbContext db) => _db = db;

    public async Task<PreventiveMaintenancePointDto> Handle(CreatePreventiveMaintenancePointCommand request, CancellationToken cancellationToken)
    {
        var exists = await _db.Set<PreventiveMaintenancePoint>().AsNoTracking()
            .Where(p => p.IsActive)
            .AnyAsync(p => p.MaintenancePointName != null && p.MaintenancePointName.ToLower() == request.MaintenancePointName.ToLower(), cancellationToken);

        if (exists)
        {
            throw new InvalidOperationException($"A preventive maintenance point with name '{request.MaintenancePointName}' already exists.");
        }

        var entity = new PreventiveMaintenancePoint
        {
            UniqueID = Guid.NewGuid(),
            MaintenancePointName = request.MaintenancePointName,
            StatusID = request.StatusID,
            IsActive = true,
            CreatedBy = request.CreateBy,
            CreatedDate = DateTime.UtcNow,
            LastModifiedBy = request.LastModifiedBy,
            LastModifiedDate = DateTime.UtcNow
        };

        _db.Set<PreventiveMaintenancePoint>().Add(entity);
        await _db.SaveChangesAsync(cancellationToken);

        return PreventiveMaintenancePointMappings.ToDto(entity);
    }
}

internal sealed class UpdatePreventiveMaintenancePointCommandHandler : IRequestHandler<UpdatePreventiveMaintenancePointCommand, PreventiveMaintenancePointDto?>
{
    private readonly IPMDbContext _db;

    public UpdatePreventiveMaintenancePointCommandHandler(IPMDbContext db) => _db = db;

    public async Task<PreventiveMaintenancePointDto?> Handle(UpdatePreventiveMaintenancePointCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.Set<PreventiveMaintenancePoint>()
            .FirstOrDefaultAsync(p => p.ID == request.Id && p.IsActive, cancellationToken);

        if (entity is null)
        {
            return null;
        }

        var duplicate = await _db.Set<PreventiveMaintenancePoint>().AsNoTracking()
            .Where(p => p.ID != request.Id && p.IsActive)
            .AnyAsync(p => p.MaintenancePointName != null && p.MaintenancePointName.ToLower() == request.MaintenancePointName.ToLower(), cancellationToken);

        if (duplicate)
        {
            throw new InvalidOperationException($"A preventive maintenance point with name '{request.MaintenancePointName}' already exists.");
        }

        entity.MaintenancePointName = request.MaintenancePointName;
        entity.StatusID = request.StatusID;
        entity.IsActive = true;
        entity.LastModifiedBy = request.LastModifiedBy;
        entity.LastModifiedDate = DateTime.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);
        return PreventiveMaintenancePointMappings.ToDto(entity);
    }
}

internal sealed class DeletePreventiveMaintenancePointCommandHandler : IRequestHandler<DeletePreventiveMaintenancePointCommand, bool>
{
    private readonly IPMDbContext _db;

    public DeletePreventiveMaintenancePointCommandHandler(IPMDbContext db) => _db = db;

    public async Task<bool> Handle(DeletePreventiveMaintenancePointCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.Set<PreventiveMaintenancePoint>()
            .FirstOrDefaultAsync(p => p.ID == request.Id, cancellationToken);

        if (entity is null)
        {
            return false;
        }

        entity.IsActive = false;
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }
}

internal sealed class GetAllPreventiveMaintenancePointsQueryHandler : IRequestHandler<GetAllPreventiveMaintenancePointsQuery, IReadOnlyCollection<PreventiveMaintenancePointDto>>
{
    private readonly IPMDbContext _db;

    public GetAllPreventiveMaintenancePointsQueryHandler(IPMDbContext db) => _db = db;

    public async Task<IReadOnlyCollection<PreventiveMaintenancePointDto>> Handle(GetAllPreventiveMaintenancePointsQuery request, CancellationToken cancellationToken) =>
        await _db.Set<PreventiveMaintenancePoint>().AsNoTracking()
            .Where(p => p.IsActive)
            .Select(p => new PreventiveMaintenancePointDto(p.ID, p.MaintenancePointName, p.StatusID))
            .ToArrayAsync(cancellationToken);
}

internal sealed class GetPreventiveMaintenancePointByIdQueryHandler : IRequestHandler<GetPreventiveMaintenancePointByIdQuery, PreventiveMaintenancePointDto?>
{
    private readonly IPMDbContext _db;

    public GetPreventiveMaintenancePointByIdQueryHandler(IPMDbContext db) => _db = db;

    public async Task<PreventiveMaintenancePointDto?> Handle(GetPreventiveMaintenancePointByIdQuery request, CancellationToken cancellationToken)
    {
        var point = await _db.Set<PreventiveMaintenancePoint>().AsNoTracking()
            .FirstOrDefaultAsync(p => p.ID == request.Id && p.IsActive, cancellationToken);

        return point is null ? null : PreventiveMaintenancePointMappings.ToDto(point);
    }
}

internal sealed class GetPreventiveMaintenancePointLookupQueryHandler : IRequestHandler<GetPreventiveMaintenancePointLookupQuery, IReadOnlyList<PreventiveMaintenancePointLookupResponse>>
{
    private readonly IPMDbContext _db;
    public GetPreventiveMaintenancePointLookupQueryHandler(IPMDbContext db) => _db = db;

    public async Task<IReadOnlyList<PreventiveMaintenancePointLookupResponse>> Handle(
        GetPreventiveMaintenancePointLookupQuery request, CancellationToken ct)
    {
        return await _db.Set<PreventiveMaintenancePoint>()
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.MaintenancePointName)
            .Select(x => new PreventiveMaintenancePointLookupResponse(x.ID, x.MaintenancePointName))
            .ToListAsync(ct);
    }
}

file static class PreventiveMaintenancePointMappings
{
    public static PreventiveMaintenancePointDto ToDto(PreventiveMaintenancePoint entity) =>
        new(entity.ID, entity.MaintenancePointName, entity.StatusID);
}
