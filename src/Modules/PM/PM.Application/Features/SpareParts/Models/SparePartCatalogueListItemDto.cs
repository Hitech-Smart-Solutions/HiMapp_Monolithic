namespace Himapp.PM.Application.Features.SpareParts.Models;

public sealed record SparePartCatalogueListItemDto(
    int ID,
    int EquipmentcategoryID,
    string? EquipmentCategoryName,
    string? BrandName,
    string? ModelName,
    string? DocumentName,
    string? DocumentPath,
    int StatusID,
    bool IsActive,
    Guid UniqueID,
    DateTime CreatedDate,
    int CreatedBy,
    DateTime? LastModifiedDate,
    int? LastModifiedBy);
