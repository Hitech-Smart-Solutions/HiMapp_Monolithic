namespace Himapp.PM.Application.Features.EquipmentChecklistPoints.Models;

public sealed record EquipmentChecklistPointListItemDto(
    int ID,
    string Description,
    bool IsActive,
    EquipmentChecklistPointType Type,
    Guid UniqueID,
    DateTime CreatedDate,
    int CreatedBy,
    DateTime LastModifiedDate,
    int LastModifiedBy);
