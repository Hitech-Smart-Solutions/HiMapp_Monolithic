using Himapp.SharedKernel.Abstractions;

namespace Himapp.PM.Domain.Entities;

/// <summary>
/// Common columns of every "equipment category to master" link table.
/// The link is created manually by the user (nothing is mapped automatically).
/// IsActive is the normal active/inactive status of the link (set by the user).
/// A link is never physically deleted: removing a master from the category sets IsDeleted = true (IsActive is left alone),
/// and adding the same master again clears IsDeleted on the same row.
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

public sealed class EquipmentCategoryPMPoint : EquipmentCategoryLinkBase
{
    public int PreventiveMaintenancePointId { get; set; }
}

public sealed class EquipmentCategoryChecklistPoint : EquipmentCategoryLinkBase
{
    public int EquipmentChecklistPointId { get; set; }
}

public sealed class EquipmentCategorySpecification : EquipmentCategoryLinkBase
{
    public int SpecificationMasterId { get; set; }
}
