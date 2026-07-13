namespace Himapp.Notifications.Models;

public sealed record DeviceToken(long UserId, string FcmToken, string Platform, DateTimeOffset LastSeenAt);
