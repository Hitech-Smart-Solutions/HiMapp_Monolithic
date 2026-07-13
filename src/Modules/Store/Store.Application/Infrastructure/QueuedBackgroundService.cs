using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Himapp.Store.Application.Infrastructure;

internal sealed class QueuedBackgroundService : BackgroundService
{
    private readonly IBackgroundTaskQueue _queue;
    private readonly ILogger<QueuedBackgroundService> _logger;

    public QueuedBackgroundService(IBackgroundTaskQueue queue, ILogger<QueuedBackgroundService> logger)
    {
        _queue = queue;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var workItem = await _queue.DequeueAsync(stoppingToken);

            try
            {
                await workItem(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Queued Store background work failed.");
            }
        }
    }
}
