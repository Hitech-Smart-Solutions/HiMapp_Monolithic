using Himapp.DynamicAudit.Models;

namespace Himapp.DynamicAudit.Abstractions;

public interface IDynamicAuditService
{
    ValueTask LogAsync(DynamicAuditLog entry, CancellationToken cancellationToken = default);
}
