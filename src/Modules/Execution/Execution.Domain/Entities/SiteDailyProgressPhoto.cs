namespace Himapp.Execution.Domain.Entities;

// EF entity for schema table "DailyProgressPhotos"  Page Name : Site DPR
public class SiteDailyProgressPhoto
{
    public int ID { get; set; }
    public Guid UniqueID { get; set; }

    public int DailyProgressID { get; set; }

    public string? FileName { get; set; }
    public string? FileType { get; set; }
    public int? FileSize { get; set; }
    public string PhotoUrl { get; set; } = string.Empty;
    public string? Caption { get; set; }

    public bool IsActive { get; set; }

    public int? CreatedBy { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
    public int? LastModifiedBy { get; set; }
    public DateTimeOffset LastModifiedDate { get; set; }
    public virtual SiteDailyProgress? DailyProgress { get; set; }
}

