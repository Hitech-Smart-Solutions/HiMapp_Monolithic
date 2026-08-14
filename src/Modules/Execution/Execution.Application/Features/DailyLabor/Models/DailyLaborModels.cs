namespace Himapp.Execution.Application.Features.DailyLabor.Models;

public sealed class DailyLaborDetailModel
{
    public int Id { get; init; }
    public Guid UniqueId { get; init; }
    public int? ContractorId { get; init; }
    public int? CategoryId { get; init; }
    public int? Skilled { get; init; }
    public int? UnSkilled { get; init; }
    public string? Remarks { get; init; }
    public int? Mat { get; init; }
    public string? ContractorName { get; init; }
    public int? ActivityId { get; init; }

    public DailyLaborDetailModel(int id, Guid uniqueId, int? contractorId, int? categoryId, int? skilled, int? unSkilled, string? remarks, int? mat, string? contractorName, int? activityId)
    {
        Id = id;
        UniqueId = uniqueId;
        ContractorId = contractorId;
        CategoryId = categoryId;
        Skilled = skilled;
        UnSkilled = unSkilled;
        Remarks = remarks;
        Mat = mat;
        ContractorName = contractorName;
        ActivityId = activityId;
    }
}

public sealed class DailyLaborModel
{
    public int Id { get; init; }
    public Guid UniqueId { get; init; }
    public string? DLRCode { get; init; }
    public string? ConstraintsAndReasons { get; set; }
    public bool? RemoveMenPower { get; init; }
    public string? ProposedActionPlan { get; set; }
    public int? CompanyId { get; init; }
    public int? ProjectId { get; init; }
    public DateTimeOffset ReportDate { get; init; }
    public string? Remarks { get; init; }
    public short? Status { get; init; }
    public bool IsActive { get; init; }
    public int CreatedBy { get; init; }
    public DateTime CreatedDate { get; init; }
    public int LastModifiedBy { get; init; }
    public DateTime LastModifiedDate { get; init; }

    public IReadOnlyCollection<DailyLaborDetailModel> Details { get; init; }

    public DailyLaborModel(int id, Guid uniqueId, string? dlrCode, int? companyId, int? projectId, DateTimeOffset reportDate, string? remarks, string? proposedActionPlan, string? constraintsAndReasons, bool? removeMenPower, short? status, bool isActive, int createdBy, DateTime createdDate, int lastModifiedBy, DateTime lastModifiedDate, IReadOnlyCollection<DailyLaborDetailModel> details)
    {
        Id = id;
        UniqueId = uniqueId;
        DLRCode = dlrCode;
        ProposedActionPlan = proposedActionPlan;
        ConstraintsAndReasons = constraintsAndReasons;
        RemoveMenPower = removeMenPower;
        CompanyId = companyId;
        ProjectId = projectId;
        ReportDate = reportDate;
        Remarks = remarks;
        Status = status;
        IsActive = isActive;
        CreatedBy = createdBy;
        CreatedDate = createdDate;
        LastModifiedBy = lastModifiedBy;
        LastModifiedDate = lastModifiedDate;
        Details = details ?? Array.Empty<DailyLaborDetailModel>();
    }
}
