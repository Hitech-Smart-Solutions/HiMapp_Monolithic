using Himapp.SharedKernel.Abstractions;

namespace Himapp.Execution.Domain.Entities;

// EF entity for schema table "Planning" Page Name : Site Execution Planning
public class Planning : BaseEntity
{
    public Planning()
    {
        PlanningDetail = new HashSet<PlanningDetail>();
        PlanningDocumentDetail = new HashSet<PlanningDocumentDetail>();
    }

    public int ProjectID { get; set; }
    public int AreaID { get; set; }
    public int PlanTypeID { get; set; }

    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }

    public string? Remarks { get; set; }
    public int StatusID { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<PlanningDetail>? PlanningDetail { get; set; }
    public virtual ICollection<PlanningDocumentDetail>? PlanningDocumentDetail { get; set; }
}