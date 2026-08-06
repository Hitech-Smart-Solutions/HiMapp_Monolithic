namespace Himapp.Execution.Application.Features.Planning.Models;

public sealed record PlanningModel(
    int Id,
    System.Guid UniqueId,
    int ProjectId,
    int PlanTypeID,
    System.DateTime StartDate,
    System.DateTime? EndDate,
    string? Remarks,
    int StatusID,
    bool IsActive,
    int? CreatedBy,
    System.DateTime CreatedDate,
    int? LastModifiedBy,
    System.DateTime LastModifiedDate,
    IReadOnlyCollection<PlanningDetailModel> Details,
    IReadOnlyCollection<PlanningDocumentDetailModel>? DocumentDetails
);

public sealed record CreatePlanningRequest(int ProjectId, int PlanTypeID, System.DateTime StartDate, System.DateTime? EndDate, string? Remarks, int CreatedBy, List<PlanningDetailRequest>? Details = null, List<PlanningDocumentDetailRequest>? docDetails = null);
public sealed record UpdatePlanningRequest(string? Remarks, int StatusID, bool IsActive, int LastModifiedBy, List<PlanningDetailRequest>? Details = null, List<PlanningDocumentDetailRequest>? docDetails = null);
