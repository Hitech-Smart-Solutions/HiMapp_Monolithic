using System.Collections.Concurrent;
using Himapp.Notifications.Models;
using Himapp.Notifications.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Himapp.Notifications;

public static class DependencyInjection
{
    public static IServiceCollection AddHimappNotifications(this IServiceCollection services)
    {
        services.AddSingleton<ConcurrentQueue<OutboxEvent>>();
        services.AddSingleton<IOutboxWriter, InMemoryOutboxWriter>();
        services.AddSingleton<INotificationDispatcher, InMemoryNotificationDispatcher>();
        return services;
    }
}
