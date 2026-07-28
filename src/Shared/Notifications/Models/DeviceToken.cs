namespace Himapp.Notifications.Models;

public sealed record DeviceToken(int UserId, string FcmToken, string Platform, DateTimeOffset LastSeenAt);
