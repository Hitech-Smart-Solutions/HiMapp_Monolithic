using Himapp.SharedKernel.Abstractions;

namespace Himapp.PM.Domain.Transfers.Events;

public sealed record EquipmentTransferCompleted(long ProjectIdValue, long TransferId, long EquipmentId) : DomainEvent(ProjectIdValue)
{
    public override string EventType => "PM.EquipmentTransferCompleted";
}
