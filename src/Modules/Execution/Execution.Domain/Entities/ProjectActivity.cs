namespace Himapp.Execution.Domain.Entities;

// EF entity for schema table "ProjectActivity"
public sealed class ProjectActivity
{
    public long Id { get; set; }
    public Guid UniqueId { get; set; }

    public long ProjectId { get; set; }
    public long ActivityId { get; set; }

    public bool Enabled { get; set; }
    public bool IsActive { get; set; }

    public long? CreatedBy { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
    public long? LastModifiedBy { get; set; }
    public DateTimeOffset LastModifiedDate { get; set; }
}

