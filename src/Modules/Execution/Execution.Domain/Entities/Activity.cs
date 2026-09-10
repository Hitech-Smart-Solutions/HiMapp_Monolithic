using Himapp.SharedKernel.Abstractions;

namespace Himapp.Execution.Domain.Entities;

// EF entity for schema table "Activity" Page Name MIS Group Activity Master
public sealed class Activity : BaseEntity
{
    public int CompanyID { get; set; }
    public string ActivityName { get; set; } = string.Empty;
    public int UOMID { get; set; }
    public decimal RevenueRate { get; set; }
    public decimal SkilledLabourRate { get; set; }
    public decimal UnSkilledLabourRate { get; set; }
    public decimal OtherLabourRate { get; set; }
    public bool OutputRequired { get; set; }
    public bool IsActive { get; set; }
}

