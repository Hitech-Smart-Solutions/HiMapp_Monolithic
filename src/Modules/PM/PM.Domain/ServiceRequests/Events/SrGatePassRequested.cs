using Himapp.SharedKernel.Abstractions;

namespace Himapp.PM.Domain.ServiceRequests.Events;

public sealed record SrGatePassRequested(int ProjectIdValue, int ServiceRequestId, int EquipmentId) : DomainEvent(ProjectIdValue)
{
    public override string EventType => "PM.SrGatePassRequested";
}
