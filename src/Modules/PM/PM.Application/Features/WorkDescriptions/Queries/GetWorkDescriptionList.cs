using Himapp.PM.Application.Features.Common;
using Himapp.PM.Application.Features.WorkDescriptions.Models;
using MediatR;

namespace Himapp.PM.Application.Features.WorkDescriptions.Queries;

public sealed record GetWorkDescriptionList(
    int Page,
    int PageSize,
    string? FilterColumn = null,
    string? FilterValue = null,
    string? SortColumn = "ID ASC",
    string? IsActive = "all")
    : IRequest<PagedResult<WorkDescriptionListItemDto>>;
