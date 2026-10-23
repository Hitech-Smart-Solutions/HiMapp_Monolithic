using Himapp.PM.Application.Features.EquipmentBreakdownReasons.Models;
using Himapp.PM.Application.Features.EquipmentBreakdownReasons.Queries;
using Himapp.PM.Domain.Entities;
using Himapp.SharedKernel.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Himapp.PM.Application.Features.EquipmentBreakdownReasons.Handlers;

internal sealed class GetEquipmentCategoryManufacturerModelsQueryHandler
    : IRequestHandler<GetEquipmentCategoryManufacturerModelsQuery, IReadOnlyList<EquipmentCategoryManufacturerModelListDto>>
{
    private readonly IPMDbContext _db;

    public GetEquipmentCategoryManufacturerModelsQueryHandler(IPMDbContext db) => _db = db;

    public async Task<IReadOnlyList<EquipmentCategoryManufacturerModelListDto>> Handle(
        GetEquipmentCategoryManufacturerModelsQuery query,
        CancellationToken cancellationToken)
    {
        var categories = await _db.Set<DynamicsEquipmentCategory>()
            .AsNoTracking()
            .Where(category => category.IsActive)
            .Select(category => new { category.ID, category.NAME, category.OBJECTTYPEID })
            .ToListAsync(cancellationToken);

        var equipment = await _db.Set<DynamicsEquipmentMaster>()
            .AsNoTracking()
            .Where(master => master.IsActive && master.OBJECTTYPE != null)
            .Select(master => new { master.OBJECTTYPE, master.MANUFACTURER, master.MODEL })
            .ToListAsync(cancellationToken);

        return categories
            .Select(category => new EquipmentCategoryManufacturerModelListDto(
                category.ID,
                category.NAME,
                equipment
                    .Where(master => string.Equals(master.OBJECTTYPE, category.OBJECTTYPEID, StringComparison.OrdinalIgnoreCase)
                        && !string.IsNullOrWhiteSpace(master.MANUFACTURER))
                    .GroupBy(master => master.MANUFACTURER!.Trim(), StringComparer.OrdinalIgnoreCase)
                    .OrderBy(group => group.Key)
                    .Select(group => new EquipmentManufacturerModelDto(
                        group.First().MANUFACTURER!.Trim(),
                        group.Select(master => master.MODEL?.Trim())
                            .Where(model => !string.IsNullOrWhiteSpace(model))
                            .Select(model => model!)
                            .Distinct(StringComparer.OrdinalIgnoreCase)
                            .OrderBy(model => model)
                            .ToArray()))
                    .ToArray()))
            .OrderBy(category => category.CategoryName)
            .ToArray();
    }
}
