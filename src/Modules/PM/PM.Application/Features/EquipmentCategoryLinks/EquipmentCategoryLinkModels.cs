namespace Himapp.PM.Application.Features.EquipmentCategoryLinks;

/// <summary>One linked master shown on a configuration tab of an equipment category.</summary>
public sealed record EquipmentCategoryLinkItemDto(
    int Id,
    int MasterId,
    string MasterName,
    string? Remarks,
    bool IsActive,
    int CreatedBy,
    DateTime CreatedDate,
    int LastModifiedBy,
    DateTime LastModifiedDate,
    EquipmentCategoryPmTrigger? PmTrigger = null);

/// <summary>
/// Default PM trigger of a PM point for equipment of the category (only used by the pm-points tab, null elsewhere):
/// a meter trigger (reading type + interval + tolerance %) and a calendar trigger (months + tolerance %).
/// </summary>
public sealed class EquipmentCategoryPmTrigger
{
    public int? ReadingTypeId { get; set; }
    public decimal? IntervalValue { get; set; }
    public decimal? ToleranceValue { get; set; }
    public int? Months { get; set; }
    public decimal? MonthsTolerance { get; set; }
}

/// <summary>An active master that can still be picked in the "Select ..." drop-down of a tab.</summary>
public sealed record EquipmentCategoryLinkOptionDto(int MasterId, string MasterName);

/// <summary>One line of the "n/6 added" panel.</summary>
public sealed record EquipmentCategoryConfigurationPointDto(string Kind, string Label, int Count, bool IsConfigured);

public sealed record EquipmentCategoryConfigurationDto(
    int EquipmentCategoryId,
    int ConfiguredCount,
    int TotalCount,
    IReadOnlyList<EquipmentCategoryConfigurationPointDto> Points);

public sealed class EquipmentCategoryLinkRequest
{
    public int MasterId { get; set; }
    public string? Remarks { get; set; }
    /// <summary>pm-points tab only. Left out (null) = keep the current trigger of an existing link.</summary>
    public EquipmentCategoryPmTrigger? PmTrigger { get; set; }
}

public sealed class EquipmentCategoryLinkUpdateRequest
{
    public string? Remarks { get; set; }
    /// <summary>When omitted the current value is kept.</summary>
    public bool? IsActive { get; set; }
    /// <summary>pm-points tab only. Left out (null) = keep the current trigger.</summary>
    public EquipmentCategoryPmTrigger? PmTrigger { get; set; }
}

public sealed class EquipmentCategoryLinkSaveRequest
{
    public List<EquipmentCategoryLinkRequest> Items { get; set; } = new();
}
