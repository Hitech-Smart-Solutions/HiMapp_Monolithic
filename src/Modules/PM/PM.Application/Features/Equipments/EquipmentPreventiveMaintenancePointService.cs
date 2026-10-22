using Himapp.PM.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Himapp.PM.Application.Features.Equipments;

internal sealed class EquipmentPreventiveMaintenancePointService : IEquipmentPreventiveMaintenancePointService
{
    private readonly IPMDbContext _db;

    public EquipmentPreventiveMaintenancePointService(IPMDbContext db) => _db = db;

    public async Task<IReadOnlyList<EquipmentPMConfigurationResponse>> GetByEquipmentAsync(
        int equipmentId,
        CancellationToken cancellationToken = default)
    {
        return await _db.Set<EquipmentPreventiveMaintenancePoints>()
            .AsNoTracking()
            .Where(x => x.EquipmentID == equipmentId && x.IsActive)
            .OrderBy(x => x.MaintenancePointID)
            .Select(x => Map(x))
            .ToListAsync(cancellationToken);
    }

    public async Task<EquipmentPMConfigurationResponse?> GetByIdAsync(
        int equipmentId,
        long id,
        CancellationToken cancellationToken = default)
    {
        return await _db.Set<EquipmentPreventiveMaintenancePoints>()
            .AsNoTracking()
            .Where(x => x.ID == id && x.EquipmentID == equipmentId && x.IsActive)
            .Select(x => Map(x))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<EquipmentPMConfigurationResponse> CreateAsync(
        int equipmentId,
        EquipmentPMConfigurationRequest request,
        CancellationToken cancellationToken = default)
    {
        ValidateSchedule(request);
        await ValidateReferencesAsync(equipmentId, request, cancellationToken);

        var duplicate = await _db.Set<EquipmentPreventiveMaintenancePoints>()
            .AnyAsync(x => x.EquipmentID == equipmentId
                        && x.MaintenancePointID == request.MaintenancePointID
                        && x.IsActive, cancellationToken);
        if (duplicate)
        {
            throw new InvalidOperationException(
                $"Maintenance point {request.MaintenancePointID} is already configured for equipment {equipmentId}.");
        }

        var now = DateTime.UtcNow;
        var entity = new EquipmentPreventiveMaintenancePoints
        {
            UniqueID = Guid.NewGuid(),
            EquipmentID = equipmentId,
            MaintenancePointID = request.MaintenancePointID,
            ItemID = request.ItemID,
            ReadingTypeID = request.ReadingTypeID,
            IntervalValue = request.IntervalValue,
            TolerenceValue = request.TolerenceValue,
            Months = request.Months,
            Remarks = request.Remarks,
            StatusID = request.StatusID,
            IsActive = true,
            CreatedBy = request.CreatedBy ?? 0,
            CreatedDate = now,
            LastModifiedBy = request.LastModifiedBy ?? request.CreatedBy ?? 0,
            LastModifiedDate = now
        };

        _db.Set<EquipmentPreventiveMaintenancePoints>().Add(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return Map(entity);
    }

    public async Task<EquipmentPMConfigurationResponse?> UpdateAsync(
        int equipmentId,
        long id,
        EquipmentPMConfigurationRequest request,
        CancellationToken cancellationToken = default)
    {
        var entity = await _db.Set<EquipmentPreventiveMaintenancePoints>()
            .FirstOrDefaultAsync(x => x.ID == id && x.EquipmentID == equipmentId && x.IsActive, cancellationToken);
        if (entity is null)
        {
            return null;
        }

        ValidateSchedule(request);
        await ValidateReferencesAsync(equipmentId, request, cancellationToken);

        var duplicate = await _db.Set<EquipmentPreventiveMaintenancePoints>()
            .AnyAsync(x => x.ID != id
                        && x.EquipmentID == equipmentId
                        && x.MaintenancePointID == request.MaintenancePointID
                        && x.IsActive, cancellationToken);
        if (duplicate)
        {
            throw new InvalidOperationException(
                $"Maintenance point {request.MaintenancePointID} is already configured for equipment {equipmentId}.");
        }

        entity.MaintenancePointID = request.MaintenancePointID;
        entity.ItemID = request.ItemID;
        entity.ReadingTypeID = request.ReadingTypeID;
        entity.IntervalValue = request.IntervalValue;
        entity.TolerenceValue = request.TolerenceValue;
        entity.Months = request.Months;
        entity.Remarks = request.Remarks;
        entity.StatusID = request.StatusID;
        entity.LastModifiedBy = request.LastModifiedBy ?? entity.LastModifiedBy;
        entity.LastModifiedDate = DateTime.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);
        return Map(entity);
    }

    public async Task<bool> DeactivateAsync(
        int equipmentId,
        long id,
        int? lastModifiedBy,
        CancellationToken cancellationToken = default)
    {
        var entity = await _db.Set<EquipmentPreventiveMaintenancePoints>()
            .FirstOrDefaultAsync(x => x.ID == id && x.EquipmentID == equipmentId && x.IsActive, cancellationToken);
        if (entity is null)
        {
            return false;
        }

        entity.IsActive = false;
        if (lastModifiedBy.HasValue)
        {
            entity.LastModifiedBy = lastModifiedBy.Value;
        }
        entity.LastModifiedDate = DateTime.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }

    private async Task ValidateReferencesAsync(
        int equipmentId,
        EquipmentPMConfigurationRequest request,
        CancellationToken cancellationToken)
    {
        var equipmentExists = await _db.Set<DynamicsEquipmentMaster>()
            .AnyAsync(x => x.ID == equipmentId, cancellationToken);
        if (!equipmentExists)
        {
            throw new InvalidOperationException($"Equipment {equipmentId} not found.");
        }

        if (request.MaintenancePointID is null)
        {
            throw new InvalidOperationException("MaintenancePointID is required.");
        }

        var maintenancePointExists = await _db.Set<PreventiveMaintenancePoint>()
            .AnyAsync(x => x.ID == request.MaintenancePointID && x.IsActive, cancellationToken);
        if (!maintenancePointExists)
        {
            throw new InvalidOperationException(
                $"Maintenance point {request.MaintenancePointID} not found or inactive.");
        }

        if (request.ReadingTypeID is int readingTypeId)
        {
            var readingTypeExists = await _db.Set<MeterReadingType>()
                .AnyAsync(x => x.ID == readingTypeId && x.IsActive, cancellationToken);
            if (!readingTypeExists)
            {
                throw new InvalidOperationException($"Reading type {readingTypeId} not found or inactive.");
            }
        }
    }

    private static void ValidateSchedule(EquipmentPMConfigurationRequest request)
    {
        if (!request.IntervalValue.HasValue && !request.Months.HasValue)
        {
            throw new InvalidOperationException(
                "At least one of IntervalValue or Months must be provided.");
        }
    }

    private static EquipmentPMConfigurationResponse Map(EquipmentPreventiveMaintenancePoints entity) =>
        new(entity.ID, entity.UniqueID, entity.EquipmentID ?? 0, entity.MaintenancePointID,
            entity.ItemID, entity.ReadingTypeID, entity.IntervalValue, entity.TolerenceValue,
            entity.Months, entity.Remarks, entity.StatusID, entity.IsActive,
            entity.CreatedAt, entity.ModifiedAt);
}
