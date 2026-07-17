namespace Himapp.Execution.Application.Features.Activities.Models;

public sealed record ActivityModel(
    long Id,
    System.Guid UniqueId,
    long CompanyId,
    string Name,
    int UomId,
    bool IsActive,
    long? CreatedBy,
    System.DateTimeOffset CreatedDate,
    long? LastModifiedBy,
    System.DateTimeOffset LastModifiedDate
);

public sealed record CreateActivityRequest(
    long CompanyId,
    string Name,
    int UomId,
    long? ProjectId // optional linking to project via ProjectActivity
);

public sealed record UpdateActivityRequest(
    string Name,
    int UomId,
    bool IsActive
);
