using MediatR;
using Himapp.PM.Application.Features.SpareParts.Models;

namespace Himapp.PM.Application.Features.SpareParts.Commands;

public sealed record UpdateSparePartCatalogueCommand(
    int Id,
    int EquipmentcategoryID,
    string? BrandName,
    string? ModelName,
    string? DocumentName,
    string? DocumentPath,
    int StatusID,
    int LastModifiedBy) : IRequest<SparePartCatalogueDto?>;
