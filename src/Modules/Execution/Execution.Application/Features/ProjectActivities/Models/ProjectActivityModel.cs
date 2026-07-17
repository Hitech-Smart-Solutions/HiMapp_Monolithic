namespace Himapp.Execution.Application.Features.ProjectActivities.Models;

public sealed record ProjectActivityModel(
    long Id,
    System.Guid UniqueId,
    long ProjectId,
    long ActivityId,
    bool IsActive,
    long? CreatedBy,
    System.DateTimeOffset CreatedDate,
    long? LastModifiedBy,
    System.DateTimeOffset LastModifiedDate
);

public sealed record CreateProjectActivityRequest(long ProjectId, long ActivityId);

public sealed record UpdateProjectActivityRequest(long ProjectId, long ActivityId, bool IsActive);
