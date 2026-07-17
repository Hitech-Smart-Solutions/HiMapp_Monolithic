namespace Himapp.Execution.Domain.Entities;

// EF entity for schema table "DailyProgressDetails"
public sealed class DailyProgressDetail
{
    public long Id { get; set; }
    public Guid UniqueId { get; set; }

    public long DailyProgressId { get; set; }
    public long ActivityId { get; set; }

    public decimal Quantity { get; set; }
    public string Uom { get; set; } = string.Empty;

    public decimal Rate { get; set; }

    // computed stored column in DB
    public decimal Amount { get; set; }

    public decimal? PlanQuantity { get; set; }

    // computed stored column in DB
    public decimal Variance { get; set; }

    public string? Remarks { get; set; }
    public bool IsActive { get; set; }

    public long? CreatedBy { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
    public long? LastModifiedBy { get; set; }
    public DateTimeOffset LastModifiedDate { get; set; }
}

