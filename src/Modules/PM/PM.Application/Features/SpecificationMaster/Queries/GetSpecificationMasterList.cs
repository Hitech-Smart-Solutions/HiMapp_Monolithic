using Himapp.PM.Application.Features.Common;
using Himapp.PM.Application.Features.SpecificationMaster.Models;
using MediatR;

namespace Himapp.PM.Application.Features.SpecificationMaster.Queries;

public sealed record GetSpecificationMasterList(
    int Page,
    int PageSize,
    string? FilterColumn = null,
    string? FilterValue = null,
    string? SortColumn = "ID ASC",
    string? IsActive = "all")
    : IRequest<PagedResult<SpecificationMasterListItemDto>>;
