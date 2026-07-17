namespace Himapp.Execution.Application.Features.Area.Models;

public sealed record AreaModel(
    long Id,
    System.Guid UniqueId,
    long ProjectId,
    string Name,
    string? Code,
    bool IsActive,
    long? CreatedBy,
    System.DateTimeOffset CreatedDate,
    long? LastModifiedBy,
    System.DateTimeOffset LastModifiedDate
);

public sealed record CreateAreaRequest(long ProjectId, string Name, string? Code);
public sealed record UpdateAreaRequest(string Name, string? Code, bool IsActive);
