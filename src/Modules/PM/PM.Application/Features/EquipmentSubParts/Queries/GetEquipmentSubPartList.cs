using Himapp.PM.Application.Features.Common;
using Himapp.PM.Application.Features.EquipmentSubParts.Models;
using MediatR;

namespace Himapp.PM.Application.Features.EquipmentSubParts.Queries;

public sealed record GetEquipmentSubPartList(
    int Page,
    int PageSize,
    string? FilterValue = null,
    string? SortColumn = "ID ASC",
    string? IsActive = "all")
    : IRequest<PagedResult<EquipmentSubPartListItemDto>>;
