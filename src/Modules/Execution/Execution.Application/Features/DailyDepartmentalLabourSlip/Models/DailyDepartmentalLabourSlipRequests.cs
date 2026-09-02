using Himapp.Workflow.Contracts.References;
using System.ComponentModel.DataAnnotations;

namespace Himapp.Execution.Application.Features.DailyDepartmentalLabourSlip.Models;

public sealed class CreateDailyDepartmentalLabourSlipRequest : IWorkflowApprovalRequest
{
    [Required]
    public int ProjectId { get; set; }

    public DateTime? SlipDate { get; set; }

    public string? IssueNumber { get; set; }

    public int? PartyID { get; set; }

    public string? Remarks { get; set; }

    public int StatusID { get; set; }

    public int CreatedBy { get; set; }

    public int LastModifiedBy { get; set; }

    public List<DailyDepartmentalLabourSlipDetailsRequest>? Details { get; set; }

    int IWorkflowApprovalRequest.StatusId => StatusID;
}

public sealed class UpdateDailyDepartmentalLabourSlipRequest : IWorkflowApprovalRequest
{
    [Required]
    public int ProjectId { get; set; }

    public DateTime? SlipDate { get; set; }

    public int? PartyID { get; set; }

    public string? Remarks { get; set; }

    public int StatusID { get; set; }

    public int LastModifiedBy { get; set; }

    public List<DailyDepartmentalLabourSlipDetailsRequest>? Details { get; set; }

    int IWorkflowApprovalRequest.StatusId => StatusID;
}

public sealed class DailyDepartmentalLabourSlipModel : IWorkflowApprovalResult
{
    public int Id { get; init; }
    public Guid UniqueId { get; init; }
    public int? ProjectId { get; init; }
    public DateTime? SlipDate { get; init; }
    public string? DDLSlipCode { get; init; }
    public string? IssueNumber { get; set; }
    public int? PartyID { get; set; }
    public string? Remarks { get; init; }
    public int StatusID { get; set; }
    public IWorkflowApprovalResult WithStatus(int statusId)
    {
        StatusID = statusId;
        return this;
    }
    public bool IsActive { get; init; }
    public int CreatedBy { get; init; }
    public DateTime CreatedDate { get; init; }
    public int LastModifiedBy { get; init; }
    public DateTime LastModifiedDate { get; init; }
    public int? IsAwaitingApprovalForId { get; init; }
    public IReadOnlyCollection<DailyDepartmentalLabourSlipDetailsModel> Details { get; init; }

    public DailyDepartmentalLabourSlipModel(int id, Guid uniqueId, int? projectId, DateTime? slipDate, string? ddlSlipCode, string? issueNumber, int? partyId, 
        string? remarks, int statusId, bool isActive, int createdBy, DateTime createdDate, int lastModifiedBy, DateTime lastModifiedDate, 
        IReadOnlyCollection<DailyDepartmentalLabourSlipDetailsModel> details, int? isAwaitingApprovalForId = null)
    {
        Id = id;
        UniqueId = uniqueId;
        ProjectId = projectId;
        SlipDate = slipDate;
        DDLSlipCode = ddlSlipCode;
        IssueNumber = issueNumber;
        PartyID = partyId;
        Remarks = remarks;
        StatusID = statusId;
        IsActive = isActive;
        CreatedBy = createdBy;
        CreatedDate = createdDate;
        LastModifiedBy = lastModifiedBy;
        LastModifiedDate = lastModifiedDate;
        Details = details ?? Array.Empty<DailyDepartmentalLabourSlipDetailsModel>();
        IsAwaitingApprovalForId = isAwaitingApprovalForId;
    }
}

public sealed class DailyDepartmentalLabourSlipDto
{
    public int Id { get; set; }
    public int ProgramId { get; set; }
}

