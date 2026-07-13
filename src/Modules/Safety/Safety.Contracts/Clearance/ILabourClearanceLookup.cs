namespace Himapp.Safety.Contracts.Clearance;

public interface ILabourClearanceLookup
{
    Task<LabourClearanceSummary?> FindAsync(long labourId, CancellationToken cancellationToken = default);
}

public sealed record LabourClearanceSummary(long LabourId, bool InductionOk, bool TestsOk, bool MedicalOk);
