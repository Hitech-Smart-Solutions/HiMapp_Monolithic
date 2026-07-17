namespace Himapp.Execution.Domain.Entities;

// EF entity for schema table "RateMaster"
public sealed class RateMaster
{
    public long Id { get; set; }
    public Guid UniqueId { get; set; }

    public long ProjectId { get; set; }
    public long ActivityId { get; set; }

    public decimal Rate { get; set; }
    public string Uom { get; set; } = string.Empty;
    public DateOnly EffectiveFrom { get; set; }

    public bool IsActive { get; set; }

    public long? CreatedBy { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
    public long? LastModifiedBy { get; set; }
    public DateTimeOffset LastModifiedDate { get; set; }
}

