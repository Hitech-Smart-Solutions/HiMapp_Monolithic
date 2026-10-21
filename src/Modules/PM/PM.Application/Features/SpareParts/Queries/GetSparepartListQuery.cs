using Himapp.PM.Application.Features.Common;
using Himapp.PM.Application.Features.SpareParts.Models;
using MediatR;

namespace Himapp.PM.Application.Features.SpareParts.Queries;

public sealed record GetSparepartListQuery(
    int Page,
    int PageSize,
    string? FilterColumn = null,
    string? FilterValue = null,
    string? SortColumn = "ID ASC",
    string? IsActive = "all")
    : IRequest<PagedResult<SparePartCatalogueListItemDto>>;
