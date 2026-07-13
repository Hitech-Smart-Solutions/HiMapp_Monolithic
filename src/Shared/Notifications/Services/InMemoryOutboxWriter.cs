using System.Collections.Concurrent;
using System.Text.Json;
using Himapp.Notifications.Models;
using Himapp.SharedKernel.Abstractions;

namespace Himapp.Notifications.Services;

public sealed class InMemoryOutboxWriter : IOutboxWriter
{
    private readonly ConcurrentQueue<OutboxEvent> _events;

    public InMemoryOutboxWriter(ConcurrentQueue<OutboxEvent> events) => _events = events;

    public Task AppendAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        var payloadJson = JsonSerializer.Serialize(domainEvent, domainEvent.GetType());
        _events.Enqueue(new OutboxEvent(domainEvent.EventType, payloadJson, domainEvent.ProjectId));
        return Task.CompletedTask;
    }
}
