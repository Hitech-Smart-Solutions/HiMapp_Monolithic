namespace Himapp.Execution.Domain.Entities;

// EF entity for schema table "PlanningDetails" Page Name : Site Execution Planning
public sealed class PlanningDetail
{
    public int ID { get; set; }
    public Guid UniqueID { get; set; }

    public int PlanningID { get; set; }
    public int AreaID { get; set; }
    public int ActivityID { get; set; }

    public decimal TargetQuantity { get; set; }
    public int UOMID { get; set; }
    public string? Remarks { get; set; }

    public bool IsActive { get; set; }

    public int? CreatedBy { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
    public int? LastModifiedBy { get; set; }
    public DateTimeOffset LastModifiedDate { get; set; }
}

