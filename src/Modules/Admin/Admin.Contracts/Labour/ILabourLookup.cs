namespace Himapp.Admin.Contracts.Labour;

public interface ILabourLookup
{
    Task<LabourSummary?> FindAsync(int labourId, CancellationToken cancellationToken = default);
}
