using Himapp.SharedKernel.Abstractions;

namespace Himapp.PM.Domain.Transfers.Events;

public sealed record EquipmentTransferCompleted(int ProjectIdValue, int TransferId, int EquipmentId) : DomainEvent(ProjectIdValue)
{
    public override string EventType => "PM.EquipmentTransferCompleted";
}
