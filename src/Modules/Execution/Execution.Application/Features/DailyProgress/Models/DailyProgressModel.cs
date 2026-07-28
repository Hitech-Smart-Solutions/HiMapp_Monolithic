using Himapp.Workflow.Models;

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
    System.DateTimeOffset LastModifiedDate
) : IRequiresApproval
{
    public string EntityName => "DailyProgress";
    public int EntityId => Id;
}

public sealed record CreateDailyProgressRequest(int ProjectId, System.DateOnly ReportDate, string? Hindrances, string? NextDayPlan, string? Remarks);
public sealed record UpdateDailyProgressRequest(string? Hindrances, string? NextDayPlan, string? Remarks, string Status, bool IsActive);
