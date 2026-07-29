namespace Himapp.Execution.Domain.Entities;

// EF entity for schema table "DailyProgressDetails"  Page Name : Site DPR
public sealed class SiteDailyProgressDetail
{
    public int ID { get; set; }
    public Guid UniqueId { get; set; }

    public int SiteDailyProgressID { get; set; }
    public int ActivityID { get; set; }

    public decimal Quantity { get; set; }
    public int UOMID { get; set; }

    public decimal Rate { get; set; }

    // computed stored column in DB
    public decimal Amount { get; set; }

    public decimal? PlanQuantity { get; set; }

    // computed stored column in DB
    public decimal Variance { get; set; }

    public string? Remarks { get; set; }
    public bool IsActive { get; set; }

    public int? CreatedBy { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
    public int? LastModifiedBy { get; set; }
    public DateTimeOffset LastModifiedDate { get; set; }
}

