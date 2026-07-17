namespace Himapp.Execution.Application.Features.Planning.Models;

public sealed record PlanningModel(
    long Id,
    System.Guid UniqueId,
    long ProjectId,
    string PlanType,
    System.DateOnly StartDate,
    System.DateOnly? EndDate,
    string? Remarks,
    string Status,
    bool IsActive,
    long? CreatedBy,
    System.DateTimeOffset CreatedDate,
    long? LastModifiedBy,
    System.DateTimeOffset LastModifiedDate
);

public sealed record CreatePlanningRequest(long ProjectId, string PlanType, System.DateOnly StartDate, System.DateOnly? EndDate, string? Remarks);
public sealed record UpdatePlanningRequest(string? Remarks, string Status, bool IsActive);
