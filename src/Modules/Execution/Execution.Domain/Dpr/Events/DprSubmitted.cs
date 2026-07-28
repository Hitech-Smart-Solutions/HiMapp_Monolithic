using Himapp.SharedKernel.Abstractions;

namespace Himapp.Execution.Domain.Dpr.Events;

public sealed record DprSubmitted(int ProjectIdValue, int DprId, DateOnly WorkDate) : DomainEvent(ProjectIdValue)
{
    public override string EventType => "Execution.DprSubmitted";
}
