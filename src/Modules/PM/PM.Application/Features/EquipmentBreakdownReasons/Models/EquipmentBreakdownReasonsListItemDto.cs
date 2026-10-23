namespace Himapp.PM.Application.Features.EquipmentBreakdownReasons.Models;

public sealed record EquipmentBreakdownReasonsListItemDto(
    int ID,
    string BreakdownReason,
    bool IsActive,
    EquipmentBreakdownReasonType Type,
    Guid UniqueID,
    DateTime CreatedDate,
    int CreatedBy,
    DateTime LastModifiedDate,
    int LastModifiedBy);
