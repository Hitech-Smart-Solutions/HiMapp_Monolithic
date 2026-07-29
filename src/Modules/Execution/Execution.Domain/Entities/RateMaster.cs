namespace Himapp.Execution.Domain.Entities;

// EF entity for schema table "RateMaster"
public sealed class RateMaster
{
    public int ID { get; set; }
    public Guid UniqueID { get; set; }

    public int ProjectID { get; set; }
    public int ActivityID { get; set; }

    public decimal Rate { get; set; }
    public int UOMID{ get; set; }
    public DateOnly EffectiveFrom { get; set; }

    public bool IsActive { get; set; }

    public int? CreatedBy { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
    public int? LastModifiedBy { get; set; }
    public DateTimeOffset LastModifiedDate { get; set; }
}

