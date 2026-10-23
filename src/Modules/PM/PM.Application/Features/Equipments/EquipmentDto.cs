namespace Himapp.PM.Application.Features.Equipments;

public sealed record EquipmentDto(long Id, long ProjectId, string AssetCode, string Name, string Category, DateOnly? MaintenanceDueOn, string Status);

public sealed record EquipmentRequest(long ProjectId, string AssetCode, string Name, string Category, DateOnly? MaintenanceDueOn);

public sealed record EquipmentPMConfigurationRequest(
    int? MaintenancePointID,
    int? ItemID,
    int? ReadingTypeID,
    decimal? IntervalValue,
    decimal? TolerenceValue,
    int? Months,
    string? Remarks,
    int StatusID,
    int? CreatedBy,
    int? LastModifiedBy);

public sealed record EquipmentPMConfigurationResponse(
    int Id,
    Guid UniqueId,
    int EquipmentID,
    int? MaintenancePointID,
    int? ItemID,
    int? ReadingTypeID,
    decimal? IntervalValue,
    decimal? TolerenceValue,
    int? Months,
    string? Remarks,
    int StatusID,
    bool IsActive,
    DateTimeOffset CreatedAt,
    DateTimeOffset? ModifiedAt);

public sealed record AwaitingPMConfigurationRow(
    int EquipmentID,
    string? AssetCode,
    string? Name,
    string? Category,
    int? DefaultPMCount,
    int ConfiguredPMCount,
    DateTimeOffset? OnboardedOn);

public sealed record MeterEpochRegisterRow(
    int EpochID,
    DateOnly OpenningDate,
    DateOnly? ClosingDate,
    decimal? ResetReadingAmount,
    decimal? OpeningReadingAmount,
    int? ReasonID,
    string? ReasonName,
    decimal? EpochUsage,
    bool IsActive,
    string? Remarks);

public sealed class MeterReplacementRequestDto
{
    public int EquipmentID { get; set; }
    public int CurrentMeterEpochID { get; set; }
    public DateTime ReplacementDate { get; set; }
    public int? MeterReadingID { get; set; }
    public decimal? ResetReadingAmount { get; set; }
    public int? ReasonID { get; set; }
    public string? Remarks { get; set; }
    public decimal? OldMeterFinalReading { get; set; }
    public decimal? OpeningReadingAmount { get; set; }
}

public sealed class MeterReplacementResponseDto
{
    public int EquipmentID { get; set; }
    public int ClosedMeterEpochID { get; set; }
    public int NewMeterEpochID { get; set; }
    public int ClosedMeterStatusID { get; set; } = 4;
    public int CurrentMeterStatusID { get; set; } = 3;
    public decimal NewMeterOpeningReadingAmount { get; set; } = 0;
    public decimal? CumulativeEquipmentHours { get; set; }
    public string Message { get; set; } = string.Empty;
}
