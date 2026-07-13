namespace Himapp.Admin.Contracts.Labour;

public sealed record LabourSummary(long LabourId, long ProjectId, string Name, string Status, long ContractorId);
