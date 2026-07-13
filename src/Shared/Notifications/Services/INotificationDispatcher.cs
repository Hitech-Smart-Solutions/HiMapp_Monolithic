namespace Himapp.Notifications.Services;

public interface INotificationDispatcher
{
    Task<int> DispatchPendingAsync(CancellationToken cancellationToken = default);
}
