namespace Himapp.Execution.Domain.Entities;

// EF entity for schema table "DailyProgressPhotos"
public sealed class DailyProgressPhoto
{
    public int ID { get; set; }
    public Guid UniqueID { get; set; }

    public int DailyProgressID { get; set; }

    public string PhotoUrl { get; set; } = string.Empty;
    public string? Caption { get; set; }

    public bool IsActive { get; set; }

    public int? CreatedBy { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
    public int? LastModifiedBy { get; set; }
    public DateTimeOffset LastModifiedDate { get; set; }
}

