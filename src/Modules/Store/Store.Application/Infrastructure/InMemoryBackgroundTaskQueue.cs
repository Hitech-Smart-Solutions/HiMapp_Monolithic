using System.Threading.Channels;

namespace Himapp.Store.Application.Infrastructure;

internal sealed class InMemoryBackgroundTaskQueue : IBackgroundTaskQueue
{
    private readonly Channel<Func<CancellationToken, ValueTask>> _queue = Channel.CreateUnbounded<Func<CancellationToken, ValueTask>>();

    public ValueTask QueueAsync(Func<CancellationToken, ValueTask> workItem, CancellationToken cancellationToken = default) =>
        _queue.Writer.WriteAsync(workItem, cancellationToken);

    public ValueTask<Func<CancellationToken, ValueTask>> DequeueAsync(CancellationToken cancellationToken) =>
        _queue.Reader.ReadAsync(cancellationToken);
}
