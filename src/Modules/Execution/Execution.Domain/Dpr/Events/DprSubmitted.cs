using Himapp.SharedKernel.Abstractions;

namespace Himapp.Execution.Domain.Dpr.Events;

public sealed record DprSubmitted(long ProjectIdValue, long DprId, DateOnly WorkDate) : DomainEvent(ProjectIdValue)
{
    public override string EventType => "Execution.DprSubmitted";
}
