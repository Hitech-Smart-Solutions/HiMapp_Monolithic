namespace Himapp.Execution.Domain.Entities;

// EF entity for schema table "DailyProgress" Page Name : Project DPR
public class DailyProgress
{

    public DailyProgress()
    {
        DailyProgressDetail = new HashSet<DailyProgressDetail>();
        DailyProgressHindrance = new HashSet<DailyProgressHindrance>();
        DailyProgressPhoto = new HashSet<DailyProgressPhoto>();
    }
    public int ID { get; set; }
    public Guid UniqueID { get; set; }

    public int ProjectID { get; set; }
    public string DPRCode { get; set; }
    public DateOnly ReportDate { get; set; }

    public string? NextDayPlan { get; set; }
    public string? Remarks { get; set; }

    public decimal TotalAmount { get; set; }
    public int StatusID { get; set; }

    public bool IsActive { get; set; } = true;

    public int CreatedBy { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
    public int LastModifiedBy { get; set; }
    public DateTimeOffset LastModifiedDate { get; set; }

    public virtual ICollection<DailyProgressDetail> DailyProgressDetail { get; set; }
    public virtual ICollection<DailyProgressHindrance> DailyProgressHindrance { get; set; }
    public virtual ICollection<DailyProgressPhoto> DailyProgressPhoto { get; set; }
}

