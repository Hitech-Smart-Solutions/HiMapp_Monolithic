using System.Threading.Channels;
using Himapp.DynamicAudit.Abstractions;
using Himapp.DynamicAudit.Models;
using Microsoft.Extensions.Logging;

namespace Himapp.DynamicAudit.Services;

public sealed class DynamicAuditService : IDynamicAuditService, IDisposable
{
    private readonly Channel<DynamicAuditLog> _channel;
    private readonly ILogger<DynamicAuditService> _logger;

    public DynamicAuditService(ILogger<DynamicAuditService> logger)
    {
        _logger = logger;
        _channel = Channel.CreateBounded<DynamicAuditLog>(new BoundedChannelOptions(1000)
        {
            FullMode = BoundedChannelFullMode.DropWrite,
            SingleReader = true,
            SingleWriter = false
        });
    }

    public ChannelReader<DynamicAuditLog> Reader => _channel.Reader;

    public ValueTask LogAsync(DynamicAuditLog entry, CancellationToken cancellationToken = default)
    {
        if (!_channel.Writer.TryWrite(entry))
        {
            _logger.LogWarning(
                "DynamicAudit channel is full. Dropping entry: User={UserId}, Request={RequestName}",
                entry.UserId, entry.RequestName);
        }

        return ValueTask.CompletedTask;
    }

    public void Dispose() => _channel.Writer.TryComplete();
}
