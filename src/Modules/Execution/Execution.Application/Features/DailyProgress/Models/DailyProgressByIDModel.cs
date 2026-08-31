using Himapp.Workflow.Contracts.References;
using System;
using System.Collections.Generic;
using System.Text;

namespace Himapp.Execution.Application.Features.DailyProgress.Models;

public sealed record DailyProgressByIDModel(
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
    int CreatedBy,
    System.DateTimeOffset CreatedDate,
    int LastModifiedBy,
    System.DateTimeOffset LastModifiedDate,
    int? NextApproverId,
    IReadOnlyCollection<DailyProgressDetailModel> Details,
    IReadOnlyCollection<DailyProgressHindranceModel> Hindrances,
    IReadOnlyCollection<DailyProgressPhotoModel> Photos
);
