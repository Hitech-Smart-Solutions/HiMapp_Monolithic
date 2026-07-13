namespace Himapp.Store.Contracts.GatePass;

public interface IGatePassLookup
{
    Task<GatePassSummary?> FindAsync(long gatePassId, CancellationToken cancellationToken = default);
}

public sealed record GatePassSummary(long GatePassId, string GatePassNo, long ProjectId, string Status);
