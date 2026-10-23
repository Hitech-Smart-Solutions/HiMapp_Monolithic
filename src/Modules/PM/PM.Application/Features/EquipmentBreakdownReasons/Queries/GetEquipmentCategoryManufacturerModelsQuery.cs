using Himapp.PM.Application.Features.EquipmentBreakdownReasons.Models;
using MediatR;

namespace Himapp.PM.Application.Features.EquipmentBreakdownReasons.Queries;

public sealed record GetEquipmentCategoryManufacturerModelsQuery
    : IRequest<IReadOnlyList<EquipmentCategoryManufacturerModelListDto>>;
