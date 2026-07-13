using Himapp.Execution.Contracts.Dpr;

namespace Himapp.Execution.Application.Lookups;

internal sealed class InMemoryDprLookup : IDprLookup
{
    public Task<DprSummary?> FindAsync(long dprId, CancellationToken cancellationToken = default) =>
        Task.FromResult<DprSummary?>(new DprSummary(dprId, 1, DateOnly.FromDateTime(DateTime.UtcNow), "Draft"));
}
