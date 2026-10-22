namespace Himapp.PM.Application.Features.WorkDescriptions.Models;

public sealed record WorkDescriptionListItemDto(
    int ID,
    string WorkDescriptionName,
    bool IsActive,
    Guid UniqueID,
    DateTime CreatedDate,
    int CreatedBy,
    DateTime LastModifiedDate,
    int LastModifiedBy);
