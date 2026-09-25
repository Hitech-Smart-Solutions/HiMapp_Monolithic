using Himapp.Workflow.Contracts.References;

namespace Himapp.Execution.Application.Features.LockRequest.Models;

public sealed class LockRequestDetailModel
{
    public int Id { get; init; }
    public Guid UniqueId { get; init; }
    public DateTime OpenDate { get; init; }
    public string? Reason { get; init; }
    public int Duration { get; init; }
    public int Counts { get; init; }
    public bool IsActive { get; init; }

    public LockRequestDetailModel(int id, Guid uniqueId, DateTime openDate, string? reason, int duration, int counts, bool isActive)
    {
        Id = id;
        UniqueId = uniqueId;
        OpenDate = openDate;
        Reason = reason;
        Duration = duration;
        Counts = counts;
        IsActive = isActive;
    }
}

public sealed class LockRequestModel : IWorkflowApprovalRequest
{
    public int Id { get; init; }
    public Guid UniqueId { get; init; }
    public string? RequestCode { get; init; }
    public DateTime RequestDate { get; init; }
    public string? Remarks { get; init; }
    public int CompanyId { get; init; }
    public int ProjectId { get; init; }
    public int ProgramId { get; init; }
    public short StateId { get; init; }
    public bool IsActive { get; init; }
    public int CreatedBy { get; init; }
    public DateTime CreatedDate { get; init; }
    public int LastModifiedBy { get; init; }
    public DateTime LastModifiedDate { get; init; }

    public IReadOnlyCollection<LockRequestDetailModel> Details { get; init; }

    int IWorkflowApprovalRequest.StatusId => StateId;

    public LockRequestModel(int id, Guid uniqueId, string? requestCode, DateTime requestDate, string? remarks, int companyId, int projectId, int programId, short stateId, bool isActive, int createdBy, DateTime createdDate, int lastModifiedBy, DateTime lastModifiedDate, IReadOnlyCollection<LockRequestDetailModel> details)
    {
        Id = id;
        UniqueId = uniqueId;
        RequestCode = requestCode;
        RequestDate = requestDate;
        Remarks = remarks;
        CompanyId = companyId;
        ProjectId = projectId;
        ProgramId = programId;
        StateId = stateId;
        IsActive = isActive;
        CreatedBy = createdBy;
        CreatedDate = createdDate;
        LastModifiedBy = lastModifiedBy;
        LastModifiedDate = lastModifiedDate;
        Details = details ?? Array.Empty<LockRequestDetailModel>();
    }
}
