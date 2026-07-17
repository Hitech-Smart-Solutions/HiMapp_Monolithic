namespace Himapp.Execution.Domain.Entities;

// EF entity for schema table "DailyLabor"
public sealed class DailyLabor
{
    public long Id { get; set; }
    public Guid UniqueId { get; set; }

    public long ProjectId { get; set; }
    public DateOnly ReportDate { get; set; }

    public string? Remarks { get; set; }
    public string Status { get; set; } = "DRAFT";

    public bool IsActive { get; set; }

    public long? CreatedBy { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
    public long? LastModifiedBy { get; set; }
    public DateTimeOffset LastModifiedDate { get; set; }
}

