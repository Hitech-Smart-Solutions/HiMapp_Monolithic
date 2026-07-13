namespace Himapp.PM.Contracts.Equipment;

public interface IEquipmentLookup
{
    Task<EquipmentSummary?> FindAsync(long equipmentId, CancellationToken cancellationToken = default);
}

public sealed record EquipmentSummary(long EquipmentId, string EquipmentNumber, string Description, string Status);
