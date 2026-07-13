using Himapp.Store.Contracts.GatePass;

namespace Himapp.Store.Application.Lookups;

internal sealed class InMemoryGatePassLookup : IGatePassLookup
{
    public Task<GatePassSummary?> FindAsync(long gatePassId, CancellationToken cancellationToken = default) =>
        Task.FromResult<GatePassSummary?>(new GatePassSummary(gatePassId, $"GP-{gatePassId:0000}", 1, "Draft"));
}
