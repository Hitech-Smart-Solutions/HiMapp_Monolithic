using Himapp.PM.Application.Features.Common;
using Himapp.PM.Application.Features.EquipmentBreakdownReasons.Models;
using MediatR;

namespace Himapp.PM.Application.Features.EquipmentBreakdownReasons.Queries;

public sealed record GetEquipmentBreakdownReasonsList(
    int Page,
    int PageSize,
    string? FilterColumn = null,
    string? FilterValue = null,
    string? SortColumn = "ID ASC",
    string? IsActive = "all")
    : IRequest<PagedResult<EquipmentBreakdownReasonsListItemDto>>;
