namespace Himapp.Execution.Application.Features.DailyProgress.Models;

public sealed class DailyProgressDetailModel
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

    public DailyProgressDetailModel(int id, Guid uniqueId, int activityId, int sectionId, decimal quantity, int? uomId, decimal rate, decimal amount, decimal? planQuantity, decimal? variance, string? remarks)
    {
        Id = id;
        UniqueId = uniqueId;
        ActivityId = activityId;
        SectionId = sectionId;
        Quantity = quantity;
        UOMID = uomId;
        Rate = rate;
        Amount = amount;
        PlanQuantity = planQuantity;
        Variance = variance;
        Remarks = remarks;
    }
}

public sealed class DailyProgressDetailRequest
{
    public int ActivityId { get; set; }
    public int SectionId { get; set; }
    public decimal Quantity { get; set; }
    public int? UOMID { get; set; }
    public decimal Rate { get; set; }
    public decimal Amount { get; init; }
    public decimal? PlanQuantity { get; set; }
    public decimal? Variance { get; init; }
    public string? Remarks { get; set; }
}
