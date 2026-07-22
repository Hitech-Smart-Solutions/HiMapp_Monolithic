namespace Himapp.Execution.Domain.Entities;

// EF entity for schema table "Activity"
public sealed class Activity
{
    public int ID { get; set; }
    public Guid UniqueID { get; set; }

    public int CompanyID { get; set; }
    public string ActivityName { get; set; } = string.Empty;

    public int UOMID;

    public bool IsActive { get; set; }

    public int CreatedBy { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
    public int LastModifiedBy { get; set; }
    public DateTimeOffset LastModifiedDate { get; set; }
}

