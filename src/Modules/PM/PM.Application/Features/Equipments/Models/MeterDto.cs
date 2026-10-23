namespace Himapp.PM.Application.Features.Equipments.Models;

public sealed record EquipmentMeterRegisterDto(
    int EquipmentID,
    decimal? StoredCumulativeValue,
    int? CurrentMeterEpochID,
    IReadOnlyList<MeterEpochRegisterRow> EpochHistory);
