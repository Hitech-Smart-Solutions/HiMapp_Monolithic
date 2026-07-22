namespace Himapp.Execution.Domain.Entities;

// EF entity for schema table "DailyLaborDetails"
public class DailyDepartmentalLabourSlipDetails
{
    public DailyDepartmentalLabourSlipDetails()
    {
        DailyDepartmentalLabourAllocationDetails = new HashSet<DailyDepartmentalLabourAllocationDetails>();
    }

    public int ID { get; set; }
    public Guid UniqueID { get; set; }

    public int? DDLSlipID { get; set; }
    public int? LabourCategoryTypeID { get; set; }
    public int? NumOfLabour { get; set; }

    public DateTime FromTime { get; set; }
    public DateTime TOTime { get; set; }
    public decimal? LunchHour { get; set; }
    public decimal? WorkingHours { get; set; }

    public int? WorkLocationID { get; set; }
    public int? ActivityCategoryID { get; set; }
    public string? ActivityDetails { get; set; }
    public int? UOMID { get; set; }
    public decimal? Quantity { get; set; }
    public int? DebitPartyID { get; set; }
    public string? Remarks { get; set; }

    public short? StateID { get; set; }
    public bool IsActive { get; set; } = true;
    public int CreatedBy { get; set; }
    public DateTime CreatedDate { get; set; }
    public int LastModifiedBy { get; set; }
    public DateTime LastModifiedDate { get; set; }

    public bool? IsLumSumWork { get; set; } = false;

    public virtual DailyDepartmentalLabourSlip? DailyDepartmentalLabourSlip { get; set; }
    public virtual ICollection<DailyDepartmentalLabourAllocationDetails> DailyDepartmentalLabourAllocationDetails { get; set; }
}

