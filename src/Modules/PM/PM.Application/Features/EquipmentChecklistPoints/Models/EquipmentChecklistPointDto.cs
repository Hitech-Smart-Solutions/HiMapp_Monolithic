namespace Himapp.PM.Application.Features.EquipmentChecklistPoints.Models;

public sealed record EquipmentChecklistPointDto(
    int Id,
    string Description,
    bool IsActive,
    EquipmentChecklistPointType Type);

public sealed record EquipmentChecklistPointRequest(
    string Description,
    bool IsActive,
    EquipmentChecklistPointType Type,
    int CreateBy,
    int LastModifiedBy);
