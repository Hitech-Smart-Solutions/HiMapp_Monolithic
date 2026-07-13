using Himapp.SharedKernel.Abstractions;

namespace Himapp.Notifications.Models;

public sealed class Notification : BaseEntity
{
    public long UserId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Body { get; init; } = string.Empty;
    public string DeepLink { get; init; } = string.Empty;
    public string Module { get; init; } = string.Empty;
    public string EventType { get; init; } = string.Empty;
    public DateTimeOffset? ReadAt { get; private set; }

    public void MarkRead(DateTimeOffset readAt) => ReadAt = readAt;
}
