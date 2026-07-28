namespace Himapp.Admin.Contracts.Labour;

public sealed record LabourSummary(int LabourId, int ProjectId, string Name, string Status, int ContractorId);
