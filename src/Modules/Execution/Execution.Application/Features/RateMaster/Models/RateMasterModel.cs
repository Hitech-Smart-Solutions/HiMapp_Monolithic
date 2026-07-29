namespace Himapp.Execution.Application.Features.RateMaster.Models;

public sealed record RateMasterModel(
    int Id,
    System.Guid UniqueId,
    int ProjectId,
    int ActivityId,
    decimal Rate,
    int UomId,
    System.DateOnly EffectiveFrom,
    bool IsActive,
    int? CreatedBy,
    System.DateTimeOffset CreatedDate,
    int? LastModifiedBy,
    System.DateTimeOffset LastModifiedDate
);

public sealed record CreateRateMasterRequest(int ProjectId, int ActivityId, decimal Rate, int UomId, System.DateOnly EffectiveFrom);
public sealed record UpdateRateMasterRequest(int ProjectId, int ActivityId, decimal Rate, int UomId, System.DateOnly EffectiveFrom, bool IsActive);
