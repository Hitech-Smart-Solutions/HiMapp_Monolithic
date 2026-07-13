namespace Himapp.PM.Application.Features.Equipments;

public sealed record EquipmentDto(long Id, long ProjectId, string AssetCode, string Name, string Category, DateOnly? MaintenanceDueOn, string Status);

public sealed record EquipmentRequest(long ProjectId, string AssetCode, string Name, string Category, DateOnly? MaintenanceDueOn);
