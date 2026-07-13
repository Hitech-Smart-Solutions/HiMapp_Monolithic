using Himapp.PM.Contracts.Equipment;

namespace Himapp.PM.Application.Lookups;

internal sealed class InMemoryEquipmentLookup : IEquipmentLookup
{
    public Task<EquipmentSummary?> FindAsync(long equipmentId, CancellationToken cancellationToken = default) =>
        Task.FromResult<EquipmentSummary?>(new EquipmentSummary(equipmentId, $"EQ-{equipmentId:0000}", "Sample Equipment", "Available"));
}
