namespace Himapp.Admin.Contracts.Contractors;

public interface IContractorLookup
{
    Task<ContractorSummary?> FindAsync(long contractorId, CancellationToken cancellationToken = default);
}

public sealed record ContractorSummary(long ContractorId, string Name, string? ContactNumber);
