using Himapp.SharedKernel.Abstractions;

namespace Himapp.PM.Domain.Entities;

/// <summary>
/// Common columns of every "equipment category to master" link table.
/// The link is created manually by the user (nothing is mapped automatically).
/// A link is never physically deleted: removing it sets IsActive = false, and adding the same master again re-activates the same row.
/// IsDeleted exists in the table but is NOT used by any business logic for now.
/// </summary>
public abstract class EquipmentCategoryLinkBase : BaseEntity
{
    public int EquipmentCategoryId { get; set; }
    public string? Remarks { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; }
}

public sealed class EquipmentCategorySubPart : EquipmentCategoryLinkBase
{
    public int EquipmentSubPartId { get; set; }
}

public sealed class EquipmentCategoryWorkDescription : EquipmentCategoryLinkBase
{
    public int WorkDescriptionId { get; set; }
}

public sealed class EquipmentCategoryBreakdownReason : EquipmentCategoryLinkBase
{
    public int EquipmentBreakdownReasonId { get; set; }
}

/// <summary>
/// PM point suggested by a category, with the DEFAULT trigger of that point for equipment of this category:
/// a meter trigger (reading type + interval + tolerance, e.g. HMR 250 +/-1.2%) and a calendar trigger (months + tolerance).
/// </summary>
public sealed class EquipmentCategoryPMPoint : EquipmentCategoryLinkBase
{
    public int PreventiveMaintenancePointId { get; set; }
    public int? ReadingTypeId { get; set; }
    public decimal? IntervalValue { get; set; }
    public decimal? ToleranceValue { get; set; }
    public int? Months { get; set; }
    public decimal? MonthsTolerance { get; set; }
}

public sealed class EquipmentCategoryChecklistPoint : EquipmentCategoryLinkBase
{
    public int EquipmentChecklistPointId { get; set; }
}

public sealed class EquipmentCategorySpecification : EquipmentCategoryLinkBase
{
    public int SpecificationMasterId { get; set; }
}
