using Himapp.Safety.Contracts.Clearance;

namespace Himapp.Safety.Application.Lookups;

internal sealed class InMemoryLabourClearanceLookup : ILabourClearanceLookup
{
    public Task<LabourClearanceSummary?> FindAsync(long labourId, CancellationToken cancellationToken = default) =>
        Task.FromResult<LabourClearanceSummary?>(new LabourClearanceSummary(labourId, true, true, false));
}
