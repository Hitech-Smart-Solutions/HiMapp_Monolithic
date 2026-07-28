namespace Himapp.Admin.Contracts.Contractors;

public interface IContractorLookup
{
    Task<ContractorSummary?> FindAsync(int contractorId, CancellationToken cancellationToken = default);
}

public sealed record ContractorSummary(int ContractorId, string Name, string? ContactNumber);
