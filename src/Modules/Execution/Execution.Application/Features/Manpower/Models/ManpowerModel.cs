namespace Himapp.Execution.Application.Features.Manpower.Models;

public sealed record ManpowerModel(
    long Id,
    System.Guid UniqueId,
    long ProjectId,
    System.DateOnly EntryDate,
    string Shift,
    string? Remarks,
    string Status,
    bool IsActive,
    long? CreatedBy,
    System.DateTimeOffset CreatedDate,
    long? LastModifiedBy,
    System.DateTimeOffset LastModifiedDate
);

public sealed record CreateManpowerRequest(long ProjectId, System.DateOnly EntryDate, string Shift, string? Remarks);
public sealed record UpdateManpowerRequest(string? Remarks, string Status, bool IsActive);
