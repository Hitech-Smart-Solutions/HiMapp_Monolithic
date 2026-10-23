using MediatR;
using Himapp.PM.Application.Features.SpareParts.Models;

namespace Himapp.PM.Application.Features.SpareParts.Commands;

public sealed record UpdateSparePartCatalogueCommand(
    int Id,
    string? DocumentName,
    string? DocumentPath,
    int LastModifiedBy) : IRequest<SparePartCatalogueDto?>;
