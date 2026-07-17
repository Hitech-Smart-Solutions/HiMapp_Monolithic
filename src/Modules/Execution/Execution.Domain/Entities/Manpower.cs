namespace Himapp.Execution.Domain.Entities;

// EF entity for schema table "Manpower"
public sealed class Manpower
{
    public long Id { get; set; }
    public Guid UniqueId { get; set; }

    public long ProjectId { get; set; }
    public DateOnly EntryDate { get; set; }
    public string Shift { get; set; } = "MORNING";

    public string? Remarks { get; set; }
    public string Status { get; set; } = "DRAFT";

    public bool IsActive { get; set; }

    public long? CreatedBy { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
    public long? LastModifiedBy { get; set; }
    public DateTimeOffset LastModifiedDate { get; set; }
}

