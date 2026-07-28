namespace Himapp.SharedKernel.Abstractions;

public interface IDomainEvent
{
    string EventType { get; }
    int? ProjectId { get; }
    DateTimeOffset OccurredAt { get; }
}
