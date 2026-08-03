namespace Himapp.Execution.Application.Features.Planning.Models;

public sealed class PlanningDetailModel
{
    public int Id { get; init; }
    public Guid UniqueId { get; init; }
    public int AreaId { get; init; }
    public int ActivityId { get; init; }
    public decimal TargetQuantity { get; init; }
    public int UomId { get; init; }
    public string? Remarks { get; init; }

    public PlanningDetailModel(int id, Guid uniqueId, int areaId, int activityId, decimal targetQuantity, int uomId, string? remarks)
    {
        Id = id;
        UniqueId = uniqueId;
        AreaId = areaId;
        ActivityId = activityId;
        TargetQuantity = targetQuantity;
        UomId = uomId;
        Remarks = remarks;
    }
}

public sealed class PlanningDetailRequest
{
    public int AreaId { get; set; }
    public int ActivityId { get; set; }
    public decimal TargetQuantity { get; set; }
    public int UomId { get; set; }
    public string? Remarks { get; set; }
}
