namespace Himapp.Execution.Domain.Entities;

// EF entity for schema table "Area"
public sealed class Area
{
    public long Id { get; set; }
    public Guid UniqueId { get; set; }

    public long ProjectId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Code { get; set; }

    public bool IsActive { get; set; }

    public long? CreatedBy { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
    public long? LastModifiedBy { get; set; }
    public DateTimeOffset LastModifiedDate { get; set; }
}

