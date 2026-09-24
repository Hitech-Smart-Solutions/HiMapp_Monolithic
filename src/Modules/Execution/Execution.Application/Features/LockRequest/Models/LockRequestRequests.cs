using Himapp.Workflow.Contracts.References;
using System.ComponentModel.DataAnnotations;

namespace Himapp.Execution.Application.Features.LockRequest.Models;

public sealed class LockRequestDetailRequest
{
    public DateTime OpenDate { get; set; }
    public string? Reason { get; set; }
    public int Duration { get; set; }
    public int Counts { get; set; }
}

public sealed class CreateLockRequestRequest : IWorkflowApprovalRequest
{
    [Required]
    public int ProjectId { get; set; }

    [Required]
    public DateTime RequestDate { get; set; }

    public string? Remarks { get; set; }
    public int? CompanyID { get; set; }
    public int ProgramID { get; set; }

    /// <summary>
    /// Status / State identifier. Use meaningful defaults in caller (e.g. Draft = 1).
    /// </summary>
    public int Status { get; set; } = 1;
    public List<LockRequestDetailRequest>? Details { get; set; }
    int IWorkflowApprovalRequest.StatusId => Status;
}

public sealed class UpdateLockRequestRequest : IWorkflowApprovalRequest
{
    [Required]
    public int ProjectId { get; set; }

    [Required]
    public DateTime RequestDate { get; set; }

    public string? Remarks { get; set; }
    public int Status { get; set; } = 1;
    public List<LockRequestDetailRequest>? Details { get; set; }
    int IWorkflowApprovalRequest.StatusId => Status;
}
