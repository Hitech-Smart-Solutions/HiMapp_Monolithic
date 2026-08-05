namespace Himapp.Execution.Application.Features.ProjectActivities.Models;

public sealed record ProjectActivityModel(
    int Id,
    System.Guid UniqueId,
    int ProjectId,
    int ActivityId,
    bool IsActive,
    bool Enabled,
    int? CreatedBy,
    System.DateTimeOffset CreatedDate,
    int? LastModifiedBy,
    System.DateTimeOffset LastModifiedDate
);

public sealed record CreateProjectActivityRequest(int ProjectId, int ActivityId, bool Enabled, int? CreatedBy, int? LastModifiedBy);

public sealed record UpdateProjectActivityRequest(int Id,int ProjectId, int ActivityId, bool Enabled, int? CreatedBy, int? LastModifiedBy);
