namespace Himapp.PM.Application.Features.EquipmentSubParts.Models;

public sealed record EquipmentSubPartListItemDto(
    int ID,
    string EquipmentSubPartName,
    bool IsActive,
    Guid UniqueID,
    DateTime CreatedDate,
    int CreatedBy,
    DateTime LastModifiedDate,
    int LastModifiedBy);
