using Himapp.SharedKernel.Abstractions;

namespace Himapp.Notifications.Services;

public interface IOutboxWriter
{
    Task AppendAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default);
}
