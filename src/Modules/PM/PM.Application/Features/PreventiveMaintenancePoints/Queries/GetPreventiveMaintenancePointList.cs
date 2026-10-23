using Himapp.PM.Application.Features.Common;
using Himapp.PM.Application.Features.PreventiveMaintenancePoints.Models;
using MediatR;

namespace Himapp.PM.Application.Features.PreventiveMaintenancePoints.Queries;

public sealed record GetPreventiveMaintenancePointList(
    int Page,
    int PageSize,
    string? FilterValue = null,
    string? SortColumn = "ID ASC",
    string? IsActive = "all")
    : IRequest<PagedResult<PreventiveMaintenancePointListItemDto>>;
