using System.Threading;
using System.Threading.Tasks;

namespace Himapp.Notifications;

/// <summary>
/// Abstraction for sending alerts/notifications to clients. Implementations
/// may send via SignalR, email, push, etc.
/// </summary>
public interface IAlertService
{
    /// <summary>
    /// Send a named event with an optional payload. If recipientUserId is provided,
    /// the implementation may target that user's group.
    /// </summary>
    Task SendAsync(string eventName, object? payload = null, int? recipientUserId = null, CancellationToken cancellationToken = default);
}
