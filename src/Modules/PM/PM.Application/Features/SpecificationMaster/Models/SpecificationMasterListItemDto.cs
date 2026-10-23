namespace Himapp.PM.Application.Features.SpecificationMaster.Models;

public sealed record SpecificationMasterListItemDto(
    int ID,
    string SpecificationName,
    bool IsActive,
    SpecificationType Type,
    Guid UniqueID,
    DateTime CreatedDate,
    int CreatedBy,
    DateTime LastModifiedDate,
    int LastModifiedBy);
