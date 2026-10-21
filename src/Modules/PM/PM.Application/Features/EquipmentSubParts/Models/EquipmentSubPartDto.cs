namespace Himapp.PM.Application.Features.EquipmentSubParts.Models;

public sealed record EquipmentSubPartDto(int Id, string EquipmentSubPartName, bool IsActive);

public sealed record EquipmentSubPartRequest(string EquipmentSubPartName, bool IsActive, int CreateBy, int LastModifiedBy);
