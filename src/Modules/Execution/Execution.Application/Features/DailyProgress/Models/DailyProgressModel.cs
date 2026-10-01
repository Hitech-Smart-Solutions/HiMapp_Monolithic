using Himapp.Workflow.Contracts.References;

namespace Himapp.Execution.Application.Features.DailyProgress.Models;

public sealed record DailyProgressModel(
    int Id,
    System.Guid UniqueId,
    int ProjectId,
    string DPRCode,
    System.DateOnly ReportDate,
    string? NextDayPlan,
    string? Remarks,
    decimal TotalAmount,
    int StatusID,
    bool IsActive,
    bool? removeMenPower,
    int CreatedBy,
    System.DateTimeOffset CreatedDate,
    int LastModifiedBy,
    System.DateTimeOffset LastModifiedDate,
    IReadOnlyCollection<DailyProgressDetailModel> Details,
    IReadOnlyCollection<DailyProgressHindranceModel> Hindrances,
    IReadOnlyCollection<DailyProgressPhotoModel> Photos
) : IWorkflowApprovalResult;

public sealed record CreateDailyProgressRequest(
    int ProjectId,
    DateOnly ReportDate,
    string? NextDayPlan,
    string? Remarks,
    decimal TotalAmount,
    int StatusID,
    int CreatedBy,
    bool? removeMenPower,
    DateTimeOffset CreatedDate,
    List<DailyProgressDetailRequest>? Details = null,
    List<DailyProgressHindranceRequest>? Hindrances = null,
    List<DailyProgressPhotoRequest>? Photos = null
) : IWorkflowApprovalRequest
{
    int IWorkflowApprovalRequest.StatusId => StatusID;
}
public sealed record UpdateDailyProgressRequest(
    int Id,
    int ProjectId,
    string? NextDayPlan,
    string? Remarks,
    decimal TotalAmount,
    int StatusID,
    int LastModifiedBy,
    DateTimeOffset LastModifiedDate,
    bool? removeMenPower,
    List<DailyProgressDetailRequest>? Details = null,
    List<DailyProgressHindranceRequest>? Hindrances = null,
    List<DailyProgressPhotoRequest>? Photos = null
) : IWorkflowApprovalRequest
{
    int IWorkflowApprovalRequest.StatusId => StatusID;
}
