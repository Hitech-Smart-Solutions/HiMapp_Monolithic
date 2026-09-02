using Himapp.SharedKernel.Abstractions;

namespace Himapp.Execution.Domain.Entities;

// EF entity for schema table "DailyProgress" Page Name : Site DPR
public class SiteDailyProgress : BaseEntity
{
    public SiteDailyProgress()
    {
        SiteDailyProgressDetail = new HashSet<SiteDailyProgressDetail>();
        SiteDailyProgressPhoto = new HashSet<SiteDailyProgressPhoto>();
        SiteDailyProgressHindrance = new HashSet<SiteDailyProgressHindrance>();
    }

    public int ProjectID { get; set; }
    public int? SectionID { get; set; }
    public DateOnly ReportDate { get; set; }
    public string? NextDayPlan { get; set; }
    public string? Remarks { get; set; }
    public decimal TotalAmount { get; set; }
    public bool IsActive { get; set; }
    public virtual ICollection<SiteDailyProgressDetail> SiteDailyProgressDetail { get; set; }
    public virtual ICollection<SiteDailyProgressPhoto> SiteDailyProgressPhoto { get; set; }
    public virtual ICollection<SiteDailyProgressHindrance> SiteDailyProgressHindrance { get; set; }
}

