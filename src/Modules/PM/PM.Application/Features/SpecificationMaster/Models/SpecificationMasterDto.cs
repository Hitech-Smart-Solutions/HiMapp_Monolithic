namespace Himapp.PM.Application.Features.SpecificationMaster.Models;

public sealed record SpecificationMasterDto(
    int Id,
    string SpecificationName,
    bool IsActive,
    SpecificationType Type);

public sealed record SpecificationMasterRequest(
    string SpecificationName,
    bool IsActive,
    SpecificationType Type,
    int CreateBy,
    int LastModifiedBy);
