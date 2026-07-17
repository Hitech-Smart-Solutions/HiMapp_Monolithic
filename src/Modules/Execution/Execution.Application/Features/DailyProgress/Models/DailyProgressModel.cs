namespace Himapp.Execution.Application.Features.DailyProgress.Models;

public sealed record DailyProgressModel(
    long Id,
    System.Guid UniqueId,
    long ProjectId,
    System.DateOnly ReportDate,
    string? Hindrances,
    string? HindranceAudioUrl,
    string? NextDayPlan,
    string? Remarks,
    decimal TotalAmount,
    string Status,
    bool IsActive,
    long? CreatedBy,
    System.DateTimeOffset CreatedDate,
    long? LastModifiedBy,
    System.DateTimeOffset LastModifiedDate
);

public sealed record CreateDailyProgressRequest(long ProjectId, System.DateOnly ReportDate, string? Hindrances, string? NextDayPlan, string? Remarks);
public sealed record UpdateDailyProgressRequest(string? Hindrances, string? NextDayPlan, string? Remarks, string Status, bool IsActive);
