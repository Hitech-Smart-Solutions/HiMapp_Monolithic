namespace Himapp.Notifications.Abstractions;

/// <summary>
/// Marker interface for responses that should trigger an automatic notification.
/// Implement on DTOs returned by MediatR handlers. The pipeline will publish
/// an alert using the provided EventName and optional RecipientUserId.
/// </summary>
public interface INotifyEvent
{
    /// <summary>Logical event name (e.g. "SiteDailyProgress.Created").</summary>
    string EventName { get; }

    /// <summary>Optional recipient user id (for targeting a single user/group).</summary>
    int? RecipientUserId { get; }

    /// <summary>Optional payload (if null pipeline will use the DTO itself).</summary>
    object? Payload { get; }
}
