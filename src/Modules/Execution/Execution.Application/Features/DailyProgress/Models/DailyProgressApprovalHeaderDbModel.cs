using System;
using System.Collections.Generic;
using System.Text;

namespace Himapp.Execution.Application.Features.DailyProgress.Models;

internal sealed record DailyProgressApprovalHeaderDbModel
{
    public int ID { get; init; }

    public Guid UniqueID { get; init; }

    public int ProjectID { get; init; }

    public string? ProjectName { get; init; }

    public string? DPRCode { get; init; }

    public DateOnly ReportDate { get; init; }

    public string? NextDayPlan { get; init; }

    public string? Remarks { get; init; }

    public decimal TotalAmount { get; init; }

    public int StatusID { get; init; }

    public bool IsActive { get; init; }

    public int CreatedBy { get; init; }

    public DateTime CreatedDate { get; init; }

    public int? LastModifiedBy { get; init; }

    public DateTime? LastModifiedDate { get; init; }

    public string? CreatedByName { get; init; }

    public int? NextApproverId { get; init; }
}