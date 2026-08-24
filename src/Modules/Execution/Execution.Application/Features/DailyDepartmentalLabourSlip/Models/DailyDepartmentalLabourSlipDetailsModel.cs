namespace Himapp.Execution.Application.Features.DailyDepartmentalLabourSlip.Models;

public sealed class DailyDepartmentalLabourSlipDetailsModel
{
    public int Id { get; init; }
    public Guid UniqueId { get; init; }
    public int? LabourCategoryTypeId { get; init; }
    public int? NumOfLabour { get; init; }
    public DateTime FromTime { get; init; }
    public DateTime ToTime { get; init; }
    public decimal? LunchHour { get; init; }
    public decimal? WorkingHours { get; init; }
    public int? WorkLocationId { get; init; }
    public int? ActivityId { get; init; }
    public string? ActivityDetails { get; init; }
    public int? UomId { get; init; }
    public decimal? Quantity { get; init; }
    public int? DebitPartyId { get; init; }
    public string? Remarks { get; init; }

    public DailyDepartmentalLabourSlipDetailsModel(int id, Guid uniqueId, int? labourCategoryTypeId, int? numOfLabour, DateTime fromTime, DateTime toTime, decimal? lunchHour, decimal? workingHours, int? workLocationId, int? activityId, string? activityDetails, int? uomId, decimal? quantity, int? debitPartyId, string? remarks)
    {
        Id = id;
        UniqueId = uniqueId;
        LabourCategoryTypeId = labourCategoryTypeId;
        NumOfLabour = numOfLabour;
        FromTime = fromTime;
        ToTime = toTime;
        LunchHour = lunchHour;
        WorkingHours = workingHours;
        WorkLocationId = workLocationId;
        ActivityId = activityId;
        ActivityDetails = activityDetails;
        UomId = uomId;
        Quantity = quantity;
        DebitPartyId = debitPartyId;
        Remarks = remarks;
    }
}

public sealed class DailyDepartmentalLabourSlipDetailsRequest
{
    public int? LabourCategoryTypeId { get; set; }
    public int? NumOfLabour { get; set; }
    public DateTime FromTime { get; set; }
    public DateTime ToTime { get; set; }
    public decimal? LunchHour { get; set; }
    public int? WorkLocationId { get; set; }
    public int? ActivityID { get; set; }
    public string? ActivityDetails { get; set; }
    public int? UomId { get; set; }
    public decimal? Quantity { get; set; }
    public int? DebitPartyId { get; set; }
    public string? Remarks { get; set; }
}
