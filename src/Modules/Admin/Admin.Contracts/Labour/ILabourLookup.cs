namespace Himapp.Admin.Contracts.Labour;

public interface ILabourLookup
{
    Task<LabourSummary?> FindAsync(long labourId, CancellationToken cancellationToken = default);
}
