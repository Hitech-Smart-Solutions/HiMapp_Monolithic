namespace Himapp.PM.Application.Features.EquipmentDocuments.Models;

public sealed record EquipmentDocumentDto(
    int Id,
    int DocumentTypeID,
    string DocumentName,
    DateTime StartDate,
    DateTime EndDate,
    bool? IsActive);

public sealed record EquipmentDocumentRequest(
    int DocumentTypeID,
    string DocumentName,
    DateTime StartDate,
    DateTime EndDate,
    bool? IsActive,
    int CreateBy,
    int LastModifiedBy);
