using Himapp.SharedKernel.Abstractions;

namespace Himapp.Execution.Domain.Entities;

// EF entity for schema table "DailyLabor"
// EF entity for schema table "DailyLabor" Page Name : Project Manpower Entry
public class DailyLabor : BaseEntity
{
    public DailyLabor()
    {
        DailyLaborDetail = new HashSet<DailyLaborDetail>();
    }

    public string? DLRCode { get; set; }
    public DateTime DLRDate { get; set; }
    public string? ConstraintsAndReasons { get; set; }
    public string? ProposedActionPlan { get; set; }
    public string? Remarks { get; set; }
    public int? CompanyID { get; set; }
    public int? ProjectID { get; set; }
    public short? StateID { get; set; }
    public bool IsActive { get; set; } = true;
    public bool? RemoveMenPower { get; set; }
    public virtual ICollection<DailyLaborDetail>? DailyLaborDetail { get; set; }
}

