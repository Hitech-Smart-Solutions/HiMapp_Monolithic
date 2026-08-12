namespace Himapp.Execution.Application.Features.Manpower.Models;

public sealed class ManpowerDetailRequest
{
    public int ContractorId { get; set; }
    public int ActivityId { get; set; }
    public int SkilledCount { get; set; }
    public int UnskilledCount { get; set; }
    public int OtherCount { get; set; }
    public bool? IsDepartment { get; set; }
}

public sealed class ManpowerDetailModel
{
    public int Id { get; init; }
    public Guid UniqueId { get; init; }
    public int ContractorId { get; init; }
    public int ActivityId { get; init; }
    public int SkilledCount { get; init; }
    public int UnskilledCount { get; init; }
    public int OtherCount { get; init; }
    public bool? IsDepartment { get; init; }
    public int TotalCount { get; init; }
    public ManpowerDetailModel(int id, Guid uniqueId, int contractorId, int activityId, int skilledCount, int unskilledCount, int otherCount, bool? isDepartment, int totalCount)
    {
        Id = id;
        UniqueId = uniqueId;
        ContractorId = contractorId;
        ActivityId = activityId;
        SkilledCount = skilledCount;
        UnskilledCount = unskilledCount;
        OtherCount = otherCount;
        IsDepartment = isDepartment;
        TotalCount = totalCount;
    }
}

public sealed class ManpowerModel
{
    public int Id { get; init; }
    public Guid UniqueId { get; init; }
    public int ProjectId { get; init; }
    public int SectionId { get; init; }
    public DateOnly EntryDate { get; init; }
    public string? Remarks { get; init; }
    public int StateId { get; init; }
    public bool IsActive { get; init; }
    public int CreatedBy { get; init; }
    public DateTimeOffset CreatedDate { get; init; }
    public int LastModifiedBy { get; init; }
    public DateTimeOffset LastModifiedDate { get; init; }

    public IReadOnlyCollection<ManpowerDetailModel> Details { get; init; }

    public ManpowerModel(int id, Guid uniqueId, int projectId, int sectionId, DateOnly entryDate, string? remarks, int stateId, bool isActive, int createdBy, DateTimeOffset createdDate, int lastModifiedBy, DateTimeOffset lastModifiedDate, IReadOnlyCollection<ManpowerDetailModel> details)
    {
        Id = id;
        UniqueId = uniqueId;
        ProjectId = projectId;
        SectionId = sectionId;
        EntryDate = entryDate;
        Remarks = remarks;
        StateId = stateId;
        IsActive = isActive;
        CreatedBy = createdBy;
        CreatedDate = createdDate;
        LastModifiedBy = lastModifiedBy;
        LastModifiedDate = lastModifiedDate;
        Details = details ?? Array.Empty<ManpowerDetailModel>();
    }
}

public sealed class CreateManpowerRequest
{
    public int ProjectId { get; set; }
    public int SectionId { get; set; }
    public DateOnly EntryDate { get; set; }
    public string? Remarks { get; set; }

    public int CreatedBy { get; init; }
    public int LastModifiedBy { get; init; }
    public List<ManpowerDetailRequest>? Details { get; set; }
}

public sealed class UpdateManpowerRequest
{
    public int SectionId { get; set; }
    public DateOnly EntryDate { get; set; }
    public string? Remarks { get; set; }
    public int StateId { get; set; }
    public bool IsActive { get; set; }

    public int LastModifiedBy { get; init; }
    public List<ManpowerDetailRequest>? Details { get; set; }
}
