namespace Himapp.Execution.Application.Features.Planning.Models;

public sealed record PlanningModel(
    int Id,
    System.Guid UniqueId,
    int ProjectId,
    string PlanType,
    System.DateOnly StartDate,
    System.DateOnly? EndDate,
    string? Remarks,
    string Status,
    bool IsActive,
    int? CreatedBy,
    System.DateTimeOffset CreatedDate,
    int? LastModifiedBy,
    System.DateTimeOffset LastModifiedDate,
    IReadOnlyCollection<PlanningDetailModel> Details
);

public sealed record CreatePlanningRequest(int ProjectId, string PlanType, System.DateOnly StartDate, System.DateOnly? EndDate, string? Remarks, List<PlanningDetailRequest>? Details = null);
public sealed record UpdatePlanningRequest(string? Remarks, string Status, bool IsActive, List<PlanningDetailRequest>? Details = null);
