using Himapp.PM.Application.Features.Common;
using Himapp.PM.Application.Features.EquipmentChecklistPoints.Models;
using MediatR;

namespace Himapp.PM.Application.Features.EquipmentChecklistPoints.Queries;

public sealed record GetEquipmentChecklistPointList(
    int Page,
    int PageSize,
    string? FilterValue = null,
    string? SortColumn = "ID ASC",
    string? IsActive = "all")
    : IRequest<PagedResult<EquipmentChecklistPointListItemDto>>;
