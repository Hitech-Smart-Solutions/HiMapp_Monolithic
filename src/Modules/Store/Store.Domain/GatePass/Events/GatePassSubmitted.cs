using Himapp.SharedKernel.Abstractions;

namespace Himapp.Store.Domain.GatePass.Events;

public sealed record GatePassSubmitted(long ProjectIdValue, long GatePassId, string GatePassNo) : DomainEvent(ProjectIdValue)
{
    public override string EventType => "Store.GatePassSubmitted";
}
