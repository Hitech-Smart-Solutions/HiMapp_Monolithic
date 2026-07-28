namespace Himapp.Notifications.Models;

public sealed record NotificationDeliveryLog(
    int? NotificationId,
    int OutboxEventId,
    NotificationChannel Channel,
    string Status,
    int Attempts,
    string? ProviderReference);
