using Microsoft.AspNetCore.SignalR;

namespace Himapp.Notifications;

public sealed class NotificationsHub : Hub
{
    public Task JoinUserGroup(long userId) =>
        Groups.AddToGroupAsync(Context.ConnectionId, $"user-{userId}");
}
