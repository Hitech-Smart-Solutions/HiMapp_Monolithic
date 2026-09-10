using Himapp.SharedKernel.Abstractions;

namespace Himapp.Execution.Domain.Entities;


public class DailyProgressPhoto : BaseEntity
{
    public DailyProgressPhoto()
    {

    }

    public int DailyProgressID { get; set; }
    public string? FileName { get; set; }
    public string? FileType { get; set; }
    public int? FileSize { get; set; }
    public string? PhotoUrl { get; set; }
    public string? Caption { get; set; }
    public bool IsActive { get; set; } = true;
    public virtual DailyProgress? DailyProgress { get; set; }
}

