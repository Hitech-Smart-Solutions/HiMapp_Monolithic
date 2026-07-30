namespace Himapp.Execution.Domain.Entities;

// EF entity for schema table "DailyProgress" Page Name : Site DPR
public class SiteDailyProgress
{
    public SiteDailyProgress()
    {
        SiteDailyProgressDetail = new HashSet<SiteDailyProgressDetail>();
        SiteDailyProgressPhoto = new HashSet<SiteDailyProgressPhoto>();
    }
    public int ID { get; set; }
    public Guid UniqueID { get; set; }

    public int ProjectID { get; set; }

    public int SectionID { get; set; }
    public DateOnly ReportDate { get; set; }

    public string? Hindrances { get; set; }
    public string? HindranceAudioUrl { get; set; }
    public string? NextDayPlan { get; set; }
    public string? Remarks { get; set; }

    public decimal TotalAmount { get; set; }

    public bool IsActive { get; set; }

    public int? CreatedBy { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
    public int? LastModifiedBy { get; set; }
    public DateTimeOffset LastModifiedDate { get; set; }

    public virtual ICollection<SiteDailyProgressDetail> SiteDailyProgressDetail { get; set; }
    public virtual ICollection<SiteDailyProgressPhoto> SiteDailyProgressPhoto { get; set; }
}

