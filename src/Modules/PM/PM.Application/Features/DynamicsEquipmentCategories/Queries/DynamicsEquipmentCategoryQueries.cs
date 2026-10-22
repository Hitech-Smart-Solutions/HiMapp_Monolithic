using Himapp.PM.Application.Features.Common;
using Himapp.PM.Application.Features.DynamicsEquipmentCategories.Models;
using MediatR;

namespace Himapp.PM.Application.Features.DynamicsEquipmentCategories.Queries;

public sealed record GetDynamicsEquipmentCategoriesQuery(ListSearchParams Params)
    : IRequest<PagedResult<DynamicsEquipmentCategoryDto>>;

public sealed record GetDynamicsEquipmentCategoryByIdQuery(int Id) : IRequest<DynamicsEquipmentCategoryDto?>;
