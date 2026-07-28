using Himapp.Audit.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Himapp.Audit.Services;

/// <summary>
/// Background service that consumes audit log entries from the channel
/// and batch-writes them to the database using a dedicated DbContext.
/// 
/// This service uses its own <see cref="AuditDbContext"/> to write logs,
/// keeping audit log writes separate from transactional DbContexts to
/// avoid contention (as per US-LOG-006/007).
/// </summary>
public sealed class BackgroundAuditConsumer : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly AuditService _auditService;
    private readonly ILogger<BackgroundAuditConsumer> _logger;

    public BackgroundAuditConsumer(
        IServiceScopeFactory scopeFactory,
        AuditService auditService,
        ILogger<BackgroundAuditConsumer> logger)
    {
        _scopeFactory = scopeFactory;
        _auditService = auditService;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("BackgroundAuditConsumer started.");

        var batch = new List<TransactionActionHistory>(50);

        await foreach (var entry in _auditService.Reader.ReadAllAsync(stoppingToken))
        {
            batch.Add(entry);

            if (batch.Count >= 50)
            {
                await FlushBatchAsync(batch, stoppingToken);
                batch.Clear();
            }
        }

        // Flush remaining entries
        if (batch.Count > 0)
        {
            await FlushBatchAsync(batch, stoppingToken);
        }

        _logger.LogInformation("BackgroundAuditConsumer stopped.");
    }

    private async Task FlushBatchAsync(List<TransactionActionHistory> batch, CancellationToken stoppingToken)
    {
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AuditDbContext>();

            db.TransactionActionHistories.AddRange(batch);
            await db.SaveChangesAsync(stoppingToken);

            _logger.LogDebug("Flushed {Count} audit log entries to database.", batch.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to flush {Count} audit log entries to database. Entries will be dropped.", batch.Count);
            // Entries are lost on failure (fire-and-forget semantics per US-LOG-006)
        }
    }
}
