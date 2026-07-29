namespace Himapp.Execution.Application.Features.ProjectActivities.Models;

public sealed record ProjectActivityModel(
    int Id,
    System.Guid UniqueId,
    int ProjectId,
    int ActivityId,
    bool IsActive,
    int? CreatedBy,
    System.DateTimeOffset CreatedDate,
    int? LastModifiedBy,
    System.DateTimeOffset LastModifiedDate
);

public sealed record CreateProjectActivityRequest(int ProjectId, int ActivityId);

public sealed record UpdateProjectActivityRequest(int ProjectId, int ActivityId, bool IsActive);
