using Himapp.SharedKernel.Abstractions;

namespace Himapp.PM.Domain.ServiceRequests.Events;

public sealed record SrGatePassRequested(long ProjectIdValue, long ServiceRequestId, long EquipmentId) : DomainEvent(ProjectIdValue)
{
    public override string EventType => "PM.SrGatePassRequested";
}
