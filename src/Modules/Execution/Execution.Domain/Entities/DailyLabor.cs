namespace Himapp.Execution.Domain.Entities;

// EF entity for schema table "DailyLabor"
// EF entity for schema table "DailyLabor" Page Name : Project Manpower Entry
public class DailyLabor
{
    public DailyLabor()
    {
        DailyLaborDetail = new HashSet<DailyLaborDetail>();
    }
    public Guid UniqueID { get; set; }
    public int ID { get; set; }
    public string? DLRCode { get; set; }
    public DateTimeOffset DLRDate { get; set; }
    public string? ConstraintsAndReasons { get; set; }
    public string? ProposedActionPlan { get; set; }
    public string? Remarks { get; set; }
    public int? CompanyID { get; set; }
    public int? ProjectID { get; set; }
    public short? StateID { get; set; }
    public bool IsActive { get; set; } = true;
    public int CreatedBy { get; set; }
    public DateTime CreatedDate { get; set; }
    public int LastModifiedBy { get; set; }
    public DateTime LastModifiedDate { get; set; }
    public bool? RemoveMenPower { get; set; }
    public virtual ICollection<DailyLaborDetail>? DailyLaborDetail { get; set; }
}

