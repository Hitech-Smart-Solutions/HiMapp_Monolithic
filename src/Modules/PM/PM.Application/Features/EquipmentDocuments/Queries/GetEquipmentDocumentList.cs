using Himapp.PM.Application.Features.Common;
using Himapp.PM.Application.Features.EquipmentDocuments.Models;
using MediatR;

namespace Himapp.PM.Application.Features.EquipmentDocuments.Queries;

public sealed record GetEquipmentDocumentList(
    int Page,
    int PageSize,
    string? FilterColumn = null,
    string? FilterValue = null,
    string? SortColumn = "ID ASC",
    string? IsActive = "all") : IRequest<PagedResult<EquipmentDocumentListItemDto>>;
