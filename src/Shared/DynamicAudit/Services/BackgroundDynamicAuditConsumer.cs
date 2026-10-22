using Himapp.DynamicAudit.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Himapp.DynamicAudit.Services;

/// <summary>
/// Background service that drains the audit channel and batch-writes
/// entries to <see cref="DynamicAuditDbContext"/> in groups of 50.
/// </summary>
public sealed class BackgroundDynamicAuditConsumer : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly DynamicAuditService _service;
    private readonly ILogger<BackgroundDynamicAuditConsumer> _logger;

    public BackgroundDynamicAuditConsumer(
        IServiceScopeFactory scopeFactory,
        DynamicAuditService service,
        ILogger<BackgroundDynamicAuditConsumer> logger)
    {
        _scopeFactory = scopeFactory;
        _service = service;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("BackgroundDynamicAuditConsumer started.");

        var batch = new List<DynamicAuditLog>(50);

        await foreach (var entry in _service.Reader.ReadAllAsync(stoppingToken))
        {
            batch.Add(entry);

            if (batch.Count >= 50)
            {
                await FlushAsync(batch, stoppingToken);
                batch.Clear();
            }
        }

        if (batch.Count > 0)
            await FlushAsync(batch, stoppingToken);

        _logger.LogInformation("BackgroundDynamicAuditConsumer stopped.");
    }

    private async Task FlushAsync(List<DynamicAuditLog> batch, CancellationToken stoppingToken)
    {
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<DynamicAuditDbContext>();
            db.DynamicAuditLogs.AddRange(batch);
            await db.SaveChangesAsync(stoppingToken);
            _logger.LogDebug("DynamicAudit: flushed {Count} entries.", batch.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                "DynamicAudit: failed to flush {Count} entries. Dropped. | {Exception}",
                batch.Count, ex.ToString().ReplaceLineEndings(" | "));
        }
    }
}
