namespace Himapp.Notifications.Models;

public sealed record NotificationDeliveryLog(
    long? NotificationId,
    long OutboxEventId,
    NotificationChannel Channel,
    string Status,
    int Attempts,
    string? ProviderReference);
