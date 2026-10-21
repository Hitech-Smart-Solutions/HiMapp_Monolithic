namespace Himapp.PM.Application.Features.EquipmentBreakdownReasons.Models;

public sealed record EquipmentBreakdownReasonsDto(
    int Id,
    string BreakdownReason,
    bool IsActive,
    EquipmentBreakdownReasonType Type);

public sealed record EquipmentBreakdownReasonsRequest(
    string BreakdownReason,
    bool IsActive,
    EquipmentBreakdownReasonType Type,
    int CreateBy,
    int LastModifiedBy);
