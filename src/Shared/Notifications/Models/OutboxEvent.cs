using Himapp.SharedKernel.Abstractions;

namespace Himapp.Notifications.Models;

public sealed class OutboxEvent : BaseEntity
{
    public OutboxEvent(string eventType, string payloadJson, long? projectId)
    {
        EventType = eventType;
        PayloadJson = payloadJson;
        ProjectId = projectId;
    }

    public string EventType { get; private set; }
    public string PayloadJson { get; private set; }
    public long? ProjectId { get; private set; }
    public DateTimeOffset OccurredAt { get; private set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? ProcessedAt { get; private set; }
    public int Attempts { get; private set; }
    public string? Error { get; private set; }

    public void MarkProcessed(DateTimeOffset processedAt) => ProcessedAt = processedAt;

    public void MarkFailed(string error)
    {
        Attempts++;
        Error = error;
    }
}
