namespace Himapp.Execution.Application.Features.DailyProgress.Models;

public sealed record DailyProgressModel(
    int Id,
    System.Guid UniqueId,
    int ProjectId,
    System.DateOnly ReportDate,
    string? NextDayPlan,
    string? Remarks,
    decimal TotalAmount,
    int StatusID,
    bool IsActive,
    int CreatedBy,
    System.DateTimeOffset CreatedDate,
    int LastModifiedBy,
    System.DateTimeOffset LastModifiedDate,
    IReadOnlyCollection<DailyProgressDetailModel> Details,
    IReadOnlyCollection<DailyProgressHindranceModel> Hindrances,
    IReadOnlyCollection<DailyProgressPhotoModel> Photos
);

public sealed record CreateDailyProgressRequest(
    int ProjectId,
    System.DateOnly ReportDate,
    string? NextDayPlan,
    string? Remarks,
    decimal TotalAmount,
    int StatusID,
    int CreatedBy,
    System.DateTimeOffset CreatedDate,
    List<DailyProgressDetailRequest>? Details = null,
    List<DailyProgressHindranceRequest>? Hindrances = null,
    List<DailyProgressPhotoRequest>? Photos = null
    );
public sealed record UpdateDailyProgressRequest(
    int Id,
    string? NextDayPlan,
    string? Remarks,
    decimal TotalAmount,
    int StatusID,
    int LastModifiedBy,
    System.DateTimeOffset LastModifiedDate,
    List<DailyProgressDetailRequest>? Details = null,
    List<DailyProgressHindranceRequest>? Hindrances = null,
    List<DailyProgressPhotoRequest>? Photos = null
    );
