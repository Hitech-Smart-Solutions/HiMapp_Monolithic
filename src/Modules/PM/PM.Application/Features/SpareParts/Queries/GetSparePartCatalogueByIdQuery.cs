using MediatR;
using Himapp.PM.Application.Features.SpareParts.Models;

namespace Himapp.PM.Application.Features.SpareParts.Queries;

public sealed record GetSparePartCatalogueByIdQuery(int Id) : IRequest<SparePartCatalogueDto?>;
