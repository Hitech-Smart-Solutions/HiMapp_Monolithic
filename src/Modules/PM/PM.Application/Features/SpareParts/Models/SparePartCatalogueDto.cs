namespace Himapp.PM.Application.Features.SpareParts.Models;

public sealed record SparePartCatalogueDto(
    int Id,
    int EquipmentcategoryID,
    string? BrandName,
    string? ModelName,
    string? DocumentName,
    string? DocumentPath,
    int StatusID,
    bool IsActive,
    int CreateBy,
    int LastModifiedBy);

public sealed record SparePartCatalogueRequest(
    int EquipmentcategoryID,
    string? BrandName,
    string? ModelName,
    string? DocumentName,
    string? DocumentPath,
    int StatusID,
    int CreateBy,
    int LastModifiedBy);
