namespace Himapp.Execution.Domain.Entities;

// EF entity for schema table "DailyProgressDetails" Page Name : Project DPR
public class DailyProgressDetail
{
    public DailyProgressDetail()
    {

    }
    public int ID { get; set; }
    public Guid UniqueID { get; set; }

    public int DailyProgressID { get; set; }
    public int ActivityID { get; set; }

    public decimal Quantity { get; set; }
    public int? UOMID { get; set; }

    public decimal Rate { get; set; }

    // computed stored column in DB
    public decimal Amount { get; set; }

    public decimal? PlanQuantity { get; set; }

    // computed stored column in DB
    public decimal? Variance { get; set; }

    public string? Remarks { get; set; }
    public bool IsActive { get; set; } = true;

    public int CreatedBy { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
    public int LastModifiedBy { get; set; }
    public DateTimeOffset LastModifiedDate { get; set; }
    public virtual DailyProgress? DailyProgress { get; set; }

}

