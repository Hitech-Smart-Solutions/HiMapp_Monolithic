namespace Himapp.Execution.Application.Features.DailyLabor.Models;

public sealed record DailyLaborModel(
    long Id,
    System.Guid UniqueId,
    long ProjectId,
    System.DateOnly ReportDate,
    string? Remarks,
    string Status,
    bool IsActive,
    long? CreatedBy,
    System.DateTimeOffset CreatedDate,
    long? LastModifiedBy,
    System.DateTimeOffset LastModifiedDate
);

public sealed record CreateDailyLaborRequest(long ProjectId, System.DateOnly ReportDate, string? Remarks);
public sealed record UpdateDailyLaborRequest(string? Remarks, string Status, bool IsActive);
