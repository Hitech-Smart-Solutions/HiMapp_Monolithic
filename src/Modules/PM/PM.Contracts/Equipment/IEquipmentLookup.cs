namespace Himapp.PM.Contracts.Equipment;

public interface IEquipmentLookup
{
    Task<EquipmentSummary?> FindAsync(int equipmentId, CancellationToken cancellationToken = default);
}

public sealed record EquipmentSummary(int EquipmentId, string EquipmentNumber, string Description, string Status);
