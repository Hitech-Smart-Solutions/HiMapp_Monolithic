namespace Himapp.Execution.Domain.Entities;

// EF entity for schema table "DailyProgressPhotos" Page Name : Project DPR
public class DailyProgressPhoto
{
    public DailyProgressPhoto()
    {

    }
    public int ID { get; set; }
    public Guid UniqueID { get; set; }

    public int DailyProgressID { get; set; }

    public string? FileName { get; set; }
    public string? FileType { get; set; }
    public int? FileSize { get; set; }
    public string? PhotoUrl { get; set; }
    public string? Caption { get; set; }

    public bool IsActive { get; set; } = true;

    public int CreatedBy { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
    public int LastModifiedBy { get; set; }
    public DateTimeOffset LastModifiedDate { get; set; }
    public virtual DailyProgress? DailyProgress { get; set; }
}

