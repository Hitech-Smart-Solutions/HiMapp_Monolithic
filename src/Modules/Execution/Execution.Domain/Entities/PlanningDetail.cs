namespace Himapp.Execution.Domain.Entities;

// EF entity for schema table "PlanningDetails"
public sealed class PlanningDetail
{
    public long Id { get; set; }
    public Guid UniqueId { get; set; }

    public long PlanningId { get; set; }
    public long AreaId { get; set; }
    public long ActivityId { get; set; }

    public decimal TargetQuantity { get; set; }
    public string Uom { get; set; } = string.Empty;
    public string? Remarks { get; set; }

    public bool IsActive { get; set; }

    public long? CreatedBy { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
    public long? LastModifiedBy { get; set; }
    public DateTimeOffset LastModifiedDate { get; set; }
}

