namespace Himapp.Execution.Contracts.Dpr;

public interface IDprLookup
{
    Task<DprSummary?> FindAsync(long dprId, CancellationToken cancellationToken = default);
}

public sealed record DprSummary(long DprId, long ProjectId, DateOnly WorkDate, string Status);
