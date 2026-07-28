using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Himapp.Notifications;

namespace Himapp.Notifications;

public sealed class DefaultAlertService : IAlertService
{
    private readonly IHubContext<NotificationsHub> _hub;

    public DefaultAlertService(IHubContext<NotificationsHub> hub) => _hub = hub;

    public async Task SendAsync(string eventName, object? payload = null, int? recipientUserId = null, CancellationToken cancellationToken = default)
    {
        if (recipientUserId.HasValue)
        {
            // Send to specific user's group
            await _hub.Clients.Group($"user-{recipientUserId.Value}").SendAsync(eventName, payload, cancellationToken);
        }
        else
        {
            // Broadcast to all connected clients
            await _hub.Clients.All.SendAsync(eventName, payload, cancellationToken);
        }
    }
}
