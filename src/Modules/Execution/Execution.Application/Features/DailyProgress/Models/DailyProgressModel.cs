namespace Himapp.Execution.Application.Features.DailyProgress.Models;

public sealed record DailyProgressModel(
    int Id,
    System.Guid UniqueId,
    int ProjectId,
    System.DateOnly ReportDate,
    string? Hindrances,
    string? HindranceAudioUrl,
    string? NextDayPlan,
    string? Remarks,
    decimal TotalAmount,
    string Status,
    bool IsActive,
    int? CreatedBy,
    System.DateTimeOffset CreatedDate,
    int? LastModifiedBy,
    System.DateTimeOffset LastModifiedDate,
    IReadOnlyCollection<DailyProgressDetailModel> Details
);

public sealed record CreateDailyProgressRequest(int ProjectId, System.DateOnly ReportDate, string? Hindrances, string? NextDayPlan, string? Remarks, List<DailyProgressDetailRequest>? Details = null);
public sealed record UpdateDailyProgressRequest(string? Hindrances, string? NextDayPlan, string? Remarks, string Status, bool IsActive, List<DailyProgressDetailRequest>? Details = null);
