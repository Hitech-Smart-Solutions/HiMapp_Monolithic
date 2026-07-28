using System.Threading.Channels;
using Himapp.Audit.Abstractions;
using Himapp.Audit.Models;
using Microsoft.Extensions.Logging;

namespace Himapp.Audit.Services;

/// <summary>
/// Channel-based audit service that queues TransactionActionHistory entries
/// without blocking the calling thread. The <see cref="BackgroundAuditConsumer"/>
/// processes the queue asynchronously.
/// </summary>
public sealed class AuditService : IAuditService, IDisposable
{
    private readonly Channel<TransactionActionHistory> _channel;
    private readonly ILogger<AuditService> _logger;

    public AuditService(ILogger<AuditService> logger)
    {
        _logger = logger;
        // Bounded channel to prevent unbounded memory growth.
        // If the channel is full, the write will wait briefly, then drop.
        _channel = Channel.CreateBounded<TransactionActionHistory>(new BoundedChannelOptions(1000)
        {
            FullMode = BoundedChannelFullMode.DropWrite,
            SingleReader = true,
            SingleWriter = false
        });
    }

    /// <summary>
    /// The channel reader — consumed by <see cref="BackgroundAuditConsumer"/>.
    /// </summary>
    public ChannelReader<TransactionActionHistory> Reader => _channel.Reader;

    public ValueTask LogAsync(
        int userId,
        int actionId,
        int programId,
        int programRowId,
        string? programRowCode,
        string? remarks,
        CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        var entry = new TransactionActionHistory
        {
            UserId = userId,
            ActionId = actionId,
            ProgramId = programId,
            ProgramRowId = programRowId,
            ProgramRowCode = programRowCode,
            Remarks = remarks ?? string.Empty,
            IsActive = true,
            CreatedBy = userId,
            CreatedDate = now,
            LastModifiedBy = userId,
            LastModifiedDate = now
        };

        // Try to write; if channel is full, log a warning and drop
        if (!_channel.Writer.TryWrite(entry))
        {
            _logger.LogWarning(
                "Audit channel is full (capacity: 1000). Dropping log entry: User={UserId}, Action={ActionId}, Entity={Entity}",
                userId, actionId, programRowCode);
        }

        return ValueTask.CompletedTask;
    }

    public void Dispose()
    {
        _channel.Writer.TryComplete();
    }
}

