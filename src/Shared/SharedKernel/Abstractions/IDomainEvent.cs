namespace Himapp.SharedKernel.Abstractions;

public interface IDomainEvent
{
    string EventType { get; }
    long? ProjectId { get; }
    DateTimeOffset OccurredAt { get; }
}
