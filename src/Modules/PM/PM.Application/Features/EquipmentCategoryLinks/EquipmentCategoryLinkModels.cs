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
    DateTime LastModifiedDate);

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
}

public sealed class EquipmentCategoryLinkUpdateRequest
{
    public string? Remarks { get; set; }
    /// <summary>When omitted the current value is kept.</summary>
    public bool? IsActive { get; set; }
}

public sealed class EquipmentCategoryLinkSaveRequest
{
    public List<EquipmentCategoryLinkRequest> Items { get; set; } = new();
}
