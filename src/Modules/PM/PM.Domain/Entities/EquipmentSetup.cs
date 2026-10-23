using Himapp.PM.Domain.Constants;
using Himapp.SharedKernel.Abstractions;

namespace Himapp.PM.Domain.Entities;

/// <summary>
/// One row per equipment: where the equipment is in the 8-step setup wizard.
/// The category only SUGGESTS PM points / specifications; whatever the user keeps is stored against the equipment itself
/// (EquipmentSetupPMPoint, EquipmentSetupSpecification, EquipmentSetupDocument).
/// </summary>
public sealed class EquipmentSetup : BaseEntity
{
    public int EquipmentId { get; set; }
    public string Status { get; set; } = EquipmentSetupStatus.Draft;
    public int CurrentStep { get; set; } = 1;
    public DateTime? CompletedAt { get; set; }
    public string? Remarks { get; set; }
    public bool IsActive { get; set; } = true;
}

/// <summary>Saved state of one wizard step of one equipment. A step without a row has not been saved yet.</summary>
public sealed class EquipmentSetupStep : BaseEntity
{
    public int EquipmentId { get; set; }
    public int StepNo { get; set; }
    public string Status { get; set; } = EquipmentSetupStepStatus.InProgress;
    public DateTime? CompletedAt { get; set; }
    public string? Remarks { get; set; }
    public bool IsActive { get; set; } = true;
}

/// <summary>
/// A PM point of one equipment. The defaults are NOT copied here: they are read from the category link (EquipmentCategoryPMPoint)
/// so only what the user configured is stored. A PM point can have a meter trigger (HMR, km, ...) and a calendar trigger (months).
/// </summary>
public sealed class EquipmentSetupPMPoint : BaseEntity
{
    public int EquipmentId { get; set; }
    public int PreventiveMaintenancePointId { get; set; }
    /// <summary>Category or Master (see EquipmentSetupSource).</summary>
    public string Source { get; set; } = EquipmentSetupSource.Category;
    public bool IsSelected { get; set; } = true;
    public int? ReadingTypeId { get; set; }
    public decimal? IntervalValue { get; set; }
    public decimal? ToleranceValue { get; set; }
    public int? Months { get; set; }
    public decimal? MonthsTolerance { get; set; }
    public string? Remarks { get; set; }
    public bool IsActive { get; set; } = true;
}

/// <summary>A specification value of one equipment.</summary>
public sealed class EquipmentSetupSpecification : BaseEntity
{
    public int EquipmentId { get; set; }
    public int SpecificationMasterId { get; set; }
    /// <summary>Category or Master (see EquipmentSetupSource).</summary>
    public string Source { get; set; } = EquipmentSetupSource.Category;
    public bool IsSelected { get; set; } = true;
    /// <summary>The value as entered (number, text or date and time, depending on the specification type).</summary>
    public string? Value { get; set; }
    public string? Remarks { get; set; }
    public bool IsActive { get; set; } = true;
}

/// <summary>
/// A document of one equipment (warranty, insurance, ...). The file itself is uploaded with the common upload API
/// (POST v1/upload/upload); the returned stored / original file names are saved here.
/// </summary>
public sealed class EquipmentSetupDocument : BaseEntity
{
    public int EquipmentId { get; set; }
    public int DocumentTypeId { get; set; }
    public string DocumentName { get; set; } = string.Empty;
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? StoredFileName { get; set; }
    public string? OriginalFileName { get; set; }
    public string? Remarks { get; set; }
    public bool IsActive { get; set; } = true;
}
