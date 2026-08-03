namespace Himapp.Execution.Application.Features.SiteDailyProgress.Models;

public sealed class SiteDailyProgressDetailModel
{
    public int Id { get; init; }
    public Guid UniqueId { get; init; }
    public int ActivityId { get; init; }
    public decimal Quantity { get; init; }
    public int UomId { get; init; }
    public decimal Rate { get; init; }
    public decimal Amount { get; init; }
    public decimal? PlanQuantity { get; init; }
    public decimal Variance { get; init; }
    public string? Remarks { get; init; }

    public SiteDailyProgressDetailModel(int id, Guid uniqueId, int activityId, decimal quantity, int uomId, decimal rate, decimal amount, decimal? planQuantity, decimal variance, string? remarks)
    {
        Id = id;
        UniqueId = uniqueId;
        ActivityId = activityId;
        Quantity = quantity;
        UomId = uomId;
        Rate = rate;
        Amount = amount;
        PlanQuantity = planQuantity;
        Variance = variance;
        Remarks = remarks;
    }
}

public sealed class SiteDailyProgressDetailRequest
{
    public int ActivityId { get; set; }
    public decimal Quantity { get; set; }
    public int UomId { get; set; }
    public decimal Rate { get; set; }
    public decimal? PlanQuantity { get; set; }
    public string? Remarks { get; set; }
}
