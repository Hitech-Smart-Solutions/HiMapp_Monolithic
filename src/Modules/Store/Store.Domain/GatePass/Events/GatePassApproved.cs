using Himapp.SharedKernel.Abstractions;

namespace Himapp.Store.Domain.GatePass.Events;

public sealed record GatePassApproved(int ProjectIdValue, int GatePassId, int? ServiceRequestId) : DomainEvent(ProjectIdValue)
{
    public override string EventType => "Store.GatePassApproved";
}
