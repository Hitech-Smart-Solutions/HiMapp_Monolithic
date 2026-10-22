namespace Himapp.PM.Application.Features.EquipmentDocuments.Models;

public sealed record EquipmentDocumentListItemDto(
    int ID,
    int DocumentTypeID,
    string DocumentName,
    string DocumentTypeName,
    DateTime StartDate,
    DateTime EndDate,
    bool? IsActive,
    Guid UniqueID,
    DateTime CreatedDate,
    int CreatedBy,
    DateTime LastModifiedDate,
    int LastModifiedBy);
