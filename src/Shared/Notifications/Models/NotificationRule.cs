namespace Himapp.Notifications.Models;

public sealed record NotificationRule(
    int RuleId,
    string EventType,
    NotificationRecipientType RecipientType,
    int? RecipientRoleId,
    long? RecipientUserId,
    string? Expression,
    NotificationChannel Channels,
    bool IsMandatory,
    bool IsActive);

public enum NotificationRecipientType
{
    Role = 1,
    User = 2,
    Expression = 3
}
