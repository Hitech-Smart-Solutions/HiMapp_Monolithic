namespace Himapp.Execution.Application.Features.RateMaster.Models;

public sealed record RateMasterModel(
    long Id,
    System.Guid UniqueId,
    long ProjectId,
    long ActivityId,
    decimal Rate,
    int UomId,
    System.DateOnly EffectiveFrom,
    bool IsActive,
    long? CreatedBy,
    System.DateTimeOffset CreatedDate,
    long? LastModifiedBy,
    System.DateTimeOffset LastModifiedDate
);

public sealed record CreateRateMasterRequest(long ProjectId, long ActivityId, decimal Rate, int UomId, System.DateOnly EffectiveFrom);
public sealed record UpdateRateMasterRequest(long ProjectId, long ActivityId, decimal Rate, int UomId, System.DateOnly EffectiveFrom, bool IsActive);
