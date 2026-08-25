using Himapp.SharedKernel.Abstractions;

namespace Himapp.Execution.Domain.Entities;

// EF entity for schema table "PlanningDetails" Page Name : Site Execution Planning
public class PlanningDetail : BaseEntity
{

    public int PlanningID { get; set; }
    public int AreaID { get; set; }
    public int ActivityID { get; set; }
    public decimal TargetQuantity { get; set; }
    public int UOMID { get; set; }
    public string? Remarks { get; set; }
    public bool IsActive { get; set; }
    public virtual Planning? Planning { get; set; }

}