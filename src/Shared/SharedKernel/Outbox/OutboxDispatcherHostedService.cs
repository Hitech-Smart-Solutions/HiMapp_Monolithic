using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace Himapp.SharedKernel.Outbox
{
    /// <summary>
    /// Background service that dispatches pending outbox messages.
    /// This implementation is intentionally simple: it reads pending messages, marks them as dispatched
    /// and relies on an injected dispatcher implementation (placeholder) to actually send messages.
    /// Replace or extend the dispatch logic to publish to Kafka/RabbitMQ or HTTP endpoints.
    /// </summary>
    public class OutboxDispatcherHostedService : BackgroundService
    {
        private readonly IServiceProvider _provider;
        private readonly ILogger<OutboxDispatcherHostedService> _logger;

        public OutboxDispatcherHostedService(IServiceProvider provider, ILogger<OutboxDispatcherHostedService> logger)
        {
            _provider = provider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Outbox dispatcher started");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _provider.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<OutboxDbContext>();

                    var pending = await db.OutboxMessages
                        .Where(m => !m.Dispatched)
                        .OrderBy(m => m.OccurredOnUtc)
                        .Take(50)
                        .ToListAsync(stoppingToken);

                    if (pending.Count == 0)
                    {
                        await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                        continue;
                    }

                    foreach (var msg in pending)
                    {
                        try
                        {
                            // TODO: replace this with real dispatcher (HTTP client, bus publisher, etc.)
                            _logger.LogInformation("Dispatching outbox message {Id} to {Destination}", msg.Id, msg.Destination);

                            // Simulate dispatch - in production send message to message broker
                            await SimulateDispatchAsync(msg, stoppingToken);

                            msg.Dispatched = true;
                            msg.DispatchedOnUtc = DateTime.UtcNow;
                        }
                        catch (Exception ex)
                        {
                            msg.DispatchAttempts++;
                            _logger.LogError(ex, "Failed to dispatch outbox message {Id}", msg.Id);
                        }
                    }

                    await db.SaveChangesAsync(stoppingToken);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    // shutting down
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Outbox dispatcher failed loop");
                    await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
                }
            }

            _logger.LogInformation("Outbox dispatcher stopping");
        }

        private Task SimulateDispatchAsync(OutboxMessage message, CancellationToken cancellationToken)
        {
            // Keep this method tiny — production should replace with a strongly-typed dispatcher service
            _logger.LogDebug("[Outbox] Simulated dispatch of message {Id} payload length {Len}", message.Id, message.Payload?.Length ?? 0);
            return Task.CompletedTask;
        }
    }
}
