namespace Himapp.PM.Application.Features.EquipmentBreakdownReasons.Models;

public sealed record EquipmentCategoryManufacturerModelListDto(
    int CategoryId,
    string CategoryName,
    IReadOnlyList<EquipmentManufacturerModelDto> Manufacturers);

public sealed record EquipmentManufacturerModelDto(
    string Manufacturer,
    IReadOnlyList<string> Models);
