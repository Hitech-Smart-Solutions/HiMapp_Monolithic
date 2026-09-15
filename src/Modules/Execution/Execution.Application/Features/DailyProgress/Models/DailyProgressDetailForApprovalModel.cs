using System;

namespace Himapp.Execution.Application.Features.DailyProgress.Models;

public sealed class DailyProgressDetailForApprovalModel
{
    public int Id { get; init; }

    public Guid UniqueId { get; init; }

    public int ActivityId { get; init; }

    public int SectionId { get; init; }

    public decimal Quantity { get; init; }

    public int? UOMID { get; init; }

    public decimal Rate { get; init; }

    public decimal Amount { get; init; }

    public decimal? PlanQuantity { get; init; }

    public decimal? Variance { get; init; }

    public string? Remarks { get; init; }

    public string? ActivityName { get; init; }

    public string? SectionName { get; init; }

    public string? UOMShortName { get; init; }
}