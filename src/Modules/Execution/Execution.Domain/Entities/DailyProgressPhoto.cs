namespace Himapp.Execution.Domain.Entities;

// EF entity for schema table "DailyProgressPhotos"
public sealed class DailyProgressPhoto
{
    public long Id { get; set; }
    public Guid UniqueId { get; set; }

    public long DailyProgressId { get; set; }

    public string PhotoUrl { get; set; } = string.Empty;
    public string? Caption { get; set; }

    public bool IsActive { get; set; }

    public long? CreatedBy { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
    public long? LastModifiedBy { get; set; }
    public DateTimeOffset LastModifiedDate { get; set; }
}

