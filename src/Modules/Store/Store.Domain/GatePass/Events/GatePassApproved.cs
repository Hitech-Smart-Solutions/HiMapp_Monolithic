using Himapp.SharedKernel.Abstractions;

namespace Himapp.Store.Domain.GatePass.Events;

public sealed record GatePassApproved(long ProjectIdValue, long GatePassId, long? ServiceRequestId) : DomainEvent(ProjectIdValue)
{
    public override string EventType => "Store.GatePassApproved";
}
