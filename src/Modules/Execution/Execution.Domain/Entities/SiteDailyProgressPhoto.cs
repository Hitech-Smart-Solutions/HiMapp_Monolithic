using Himapp.SharedKernel.Abstractions;

namespace Himapp.Execution.Domain.Entities;

// EF entity for schema table "DailyProgressPhotos"  Page Name : Site DPR
public class SiteDailyProgressPhoto : BaseEntity
{

    public int DailyProgressID { get; set; }

    public string? FileName { get; set; }
    public string? FileType { get; set; }
    public int? FileSize { get; set; }
    public string PhotoUrl { get; set; } = string.Empty;
    public string? Caption { get; set; }
    public bool IsActive { get; set; }
    public virtual SiteDailyProgress? DailyProgress { get; set; }
}

