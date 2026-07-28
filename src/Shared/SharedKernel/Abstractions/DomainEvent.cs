namespace Himapp.SharedKernel.Abstractions;

public abstract record DomainEvent(int? ProjectId) : IDomainEvent
{
    public abstract string EventType { get; }
    public DateTimeOffset OccurredAt { get; init; } = DateTimeOffset.UtcNow;
}
