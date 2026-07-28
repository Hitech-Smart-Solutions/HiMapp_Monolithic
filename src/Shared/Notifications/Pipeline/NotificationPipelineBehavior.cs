using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Himapp.Notifications.Abstractions;

namespace Himapp.Notifications.Pipeline;

public sealed class NotificationPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IAlertService _alertService;

    public NotificationPipelineBehavior(IAlertService alertService) => _alertService = alertService;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var response = await next();

        try
        {
            // 1) If response implements INotifyEvent, use that
            if (response is INotifyEvent notify)
            {
                var payload = notify.Payload ?? response!;
                await _alertService.SendAsync(notify.EventName, payload, notify.RecipientUserId, cancellationToken);
                return response;
            }

            // 2) If request type has NotifyOnSuccessAttribute, publish with that name and payload = response
            var attr = request.GetType().GetCustomAttribute<NotifyOnSuccessAttribute>(inherit: true);
            if (attr != null)
            {
                await _alertService.SendAsync(attr.EventName, response, null, cancellationToken);
            }
        }
        catch
        {
            // Swallow exceptions to avoid breaking main flow. Alerting is best-effort.
        }

        return response;
    }
}
