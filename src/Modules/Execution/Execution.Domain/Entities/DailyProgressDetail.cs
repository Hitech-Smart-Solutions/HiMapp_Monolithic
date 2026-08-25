using Himapp.SharedKernel.Abstractions;

namespace Himapp.Execution.Domain.Entities;

// EF entity for schema table "DailyProgressDetails" Page Name : Project DPR
public class DailyProgressDetail : BaseEntity
{
    public DailyProgressDetail()
    {

    }

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
    public virtual DailyProgress? DailyProgress { get; set; }

}

