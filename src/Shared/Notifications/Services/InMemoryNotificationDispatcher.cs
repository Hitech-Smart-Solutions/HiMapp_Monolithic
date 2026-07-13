using System.Collections.Concurrent;
using Himapp.Notifications.Models;
using Himapp.SharedKernel.Abstractions;

namespace Himapp.Notifications.Services;

public sealed class InMemoryNotificationDispatcher : INotificationDispatcher
{
    private readonly ConcurrentQueue<OutboxEvent> _events;
    private readonly IClock _clock;

    public InMemoryNotificationDispatcher(ConcurrentQueue<OutboxEvent> events, IClock clock)
    {
        _events = events;
        _clock = clock;
    }

    public Task<int> DispatchPendingAsync(CancellationToken cancellationToken = default)
    {
        var dispatched = 0;
        while (_events.TryDequeue(out var outboxEvent))
        {
            outboxEvent.MarkProcessed(_clock.UtcNow);
            dispatched++;
        }

        return Task.FromResult(dispatched);
    }
}
