namespace Himapp.Execution.Application.Features.DailyLabor.Models;

public sealed record DailyLaborModel(
    long Id,
    System.Guid UniqueId,
    int? ProjectId,
    System.DateTime? ReportDate,
    string? Remarks,
    short? Status,
    bool IsActive,
    int? CreatedBy,
    System.DateTime CreatedDate,
    int? LastModifiedBy,
    System.DateTime LastModifiedDate
);

public sealed record CreateDailyLaborRequest(int ProjectID, System.DateTime? SlipDate, string? Remarks);
public sealed record UpdateDailyLaborRequest(string? Remarks, short? StateID, bool IsActive);
