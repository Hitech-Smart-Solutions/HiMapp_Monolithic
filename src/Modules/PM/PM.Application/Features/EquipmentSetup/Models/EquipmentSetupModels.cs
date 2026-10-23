using Himapp.PM.Application.Features.DynamicsEquipmentMasters.Models;

namespace Himapp.PM.Application.Features.EquipmentSetup.Models;

// ---------------------------------------------------------------- overview (left panel of the wizard)

public sealed record EquipmentSetupStepDto(int StepNo, string Key, string Title, string Hint, string Status);

public sealed record EquipmentSetupOverviewDto(
    int EquipmentId,
    string EquipmentCode,
    string EquipmentName,
    string SetupStatus,
    int CurrentStep,
    int TotalSteps,
    IReadOnlyList<EquipmentSetupStepDto> Steps);

/// <summary>Returned by every Save / Confirm call: the step that was saved and what the wizard looks like now.</summary>
public sealed record EquipmentSetupStepResultDto(
    int StepNo,
    string StepStatus,
    string SetupStatus,
    int CurrentStep,
    IReadOnlyList<EquipmentSetupStepDto> Steps);

// ---------------------------------------------------------------- step 1: review equipment details (read only, D365)

public sealed record EquipmentDetailsStepDto(
    DynamicsEquipmentMasterDto Equipment,
    int? CategoryId,
    string? CategoryName);

// ---------------------------------------------------------------- step 2: update equipment information

public sealed record EquipmentInformationDto(
    string? PoNumber,
    string? VendorName,
    decimal? AcquisitionCost,
    DateTime? AcquisitionDate,
    string? EquipmentCategory,
    string? PreferredLocation);

public sealed class EquipmentInformationRequest
{
    public string? PoNumber { get; set; }
    public string? VendorName { get; set; }
    public decimal? AcquisitionCost { get; set; }
    public DateTime? AcquisitionDate { get; set; }
}

// ---------------------------------------------------------------- step 3: meter and fuel

public sealed record EquipmentMeterFuelDto(
    bool LogbookRequired,
    bool FuelStandardRequired,
    int? ReadingTypeId,
    DateTime? OpeningDate,
    decimal? OpeningReading,
    float? FuelStandard);

public sealed class EquipmentMeterFuelRequest
{
    public int? ReadingTypeId { get; set; }
    public DateTime? OpeningDate { get; set; }
    public decimal? OpeningReading { get; set; }
    public float? FuelStandard { get; set; }
}

// ---------------------------------------------------------------- step 4: documents

public sealed record EquipmentDocumentDto(
    int Id,
    int DocumentTypeId,
    string DocumentTypeName,
    string DocumentName,
    DateTime? StartDate,
    DateTime? EndDate,
    string? StoredFileName,
    string? OriginalFileName,
    string? Remarks);

public sealed record EquipmentDocumentsStepDto(int Count, int WithAttachments, IReadOnlyList<EquipmentDocumentDto> Items);

public sealed class EquipmentDocumentRequest
{
    /// <summary>Id of an existing document to change; leave out (or 0) for a new one.</summary>
    public int? Id { get; set; }
    public int DocumentTypeId { get; set; }
    public string DocumentName { get; set; } = string.Empty;
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? StoredFileName { get; set; }
    public string? OriginalFileName { get; set; }
    public string? Remarks { get; set; }
}

public sealed class EquipmentDocumentsSaveRequest
{
    public List<EquipmentDocumentRequest> Items { get; set; } = new();
}

// ---------------------------------------------------------------- step 5: preventive maintenance

/// <summary>One trigger of a PM point. Default comes from the category, Configured is what this equipment uses.</summary>
public sealed record EquipmentPmTriggerDto(
    decimal? DefaultValue,
    decimal? DefaultTolerance,
    decimal? ConfiguredValue,
    decimal? ConfiguredTolerance);

public sealed record EquipmentPmPointDto(
    int PmPointId,
    string PmPointName,
    string Source,
    bool IsSelected,
    int? ReadingTypeId,
    string? ReadingTypeName,
    EquipmentPmTriggerDto? MeterTrigger,
    EquipmentPmTriggerDto? MonthsTrigger,
    string? Remarks,
    /// <summary>Not calculated yet (needs the current meter reading of the equipment).</summary>
    string? NextDue);

public sealed record EquipmentPmSummaryDto(int CategorySelected, int FromMaster, int Selected, int Total);

public sealed record EquipmentPmStepDto(
    int? CategoryId,
    string? CategoryName,
    EquipmentPmSummaryDto Summary,
    IReadOnlyList<EquipmentPmPointDto> CategoryPoints,
    IReadOnlyList<EquipmentPmPointDto> MasterPoints);

public sealed class EquipmentPmMeterRequest
{
    public int? ReadingTypeId { get; set; }
    public decimal? ConfiguredValue { get; set; }
    public decimal? ConfiguredTolerance { get; set; }
}

public sealed class EquipmentPmMonthsRequest
{
    public decimal? ConfiguredValue { get; set; }
    public decimal? ConfiguredTolerance { get; set; }
}

public sealed class EquipmentPmPointRequest
{
    public int PmPointId { get; set; }
    /// <summary>Category (suggested by the category) or Master (added by hand). Default Category.</summary>
    public string Source { get; set; } = "Category";
    public bool IsSelected { get; set; } = true;
    public EquipmentPmMeterRequest? MeterTrigger { get; set; }
    public EquipmentPmMonthsRequest? MonthsTrigger { get; set; }
    public string? Remarks { get; set; }
}

public sealed class EquipmentPmSaveRequest
{
    public List<EquipmentPmPointRequest> Items { get; set; } = new();
}

public sealed record EquipmentOptionDto(int Id, string Name);

// ---------------------------------------------------------------- step 6: specifications

public sealed record EquipmentSpecificationDto(
    int SpecificationId,
    string SpecificationName,
    /// <summary>The Type of the specification master (decides the input: number, text, date and time).</summary>
    int SpecificationType,
    string Source,
    bool IsSelected,
    string? Value,
    string? Remarks);

public sealed record EquipmentSpecificationSummaryDto(int CategorySelected, int FromMaster, int Selected, int Total);

public sealed record EquipmentSpecificationsStepDto(
    int? CategoryId,
    string? CategoryName,
    EquipmentSpecificationSummaryDto Summary,
    IReadOnlyList<EquipmentSpecificationDto> CategorySpecifications,
    IReadOnlyList<EquipmentSpecificationDto> MasterSpecifications);

public sealed class EquipmentSpecificationRequest
{
    public int SpecificationId { get; set; }
    public string Source { get; set; } = "Category";
    public bool IsSelected { get; set; } = true;
    public string? Value { get; set; }
    public string? Remarks { get; set; }
}

public sealed class EquipmentSpecificationsSaveRequest
{
    public List<EquipmentSpecificationRequest> Items { get; set; } = new();
}

public sealed record EquipmentSpecificationOptionDto(int Id, string Name, int Type);

// ---------------------------------------------------------------- step 7 / 8: review and completion

public sealed record EquipmentReviewDto(
    string EquipmentCode,
    string EquipmentName,
    string? EquipmentCategory,
    string? PreferredLocation,
    EquipmentInformationDto Procurement,
    string MeterStatus,
    string FuelStatus,
    int DocumentRecords,
    DateTime? LatestDocumentEnd,
    int CategorySelectedPm,
    int PmFromMaster,
    int PmTriggers,
    int PmOverrides,
    int TotalSpecifications,
    IReadOnlyList<EquipmentSetupStepDto> Steps);

public sealed record EquipmentCompletionDto(
    string EquipmentCode,
    string SetupStatus,
    DateTime? CompletedAt,
    string MeterBaseline,
    string FuelStandard,
    int PmPoints,
    int PmFromCategory,
    int PmFromMaster);
