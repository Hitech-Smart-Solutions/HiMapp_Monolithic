using System;

namespace Himapp.Notifications;

/// <summary>
/// Attribute to decorate MediatR request/command types to indicate that
/// a named notification should be emitted on successful completion.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class NotifyOnSuccessAttribute : Attribute
{
    public string EventName { get; }

    public NotifyOnSuccessAttribute(string eventName) => EventName = eventName;
}
