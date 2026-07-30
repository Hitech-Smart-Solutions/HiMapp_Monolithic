namespace Himapp.Execution.Domain.Entities;

// EF entity for schema table "Planning" Page Name : Site Execution Planning
public class Planning 
{
    public Planning()
    {
        PlanningDetail = new HashSet<PlanningDetail>();
    }
    public int ID { get; set; }
    public Guid UniqueID { get; set; }

    public int ProjectID { get; set; }
    public string PlanType { get; set; } = "DAILY";

    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }

    public string? Remarks { get; set; }
    public string Status { get; set; } = "DRAFT";

    public bool IsActive { get; set; }

    public int? CreatedBy { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
    public int? LastModifiedBy { get; set; }
    public DateTimeOffset LastModifiedDate { get; set; }
    public virtual ICollection<PlanningDetail>? PlanningDetail { get; set; }
}

