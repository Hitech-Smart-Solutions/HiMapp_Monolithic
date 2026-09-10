using Himapp.SharedKernel.Abstractions;

namespace Himapp.Execution.Domain.Entities;

// EF entity for schema table "ProjectActivity"
public sealed class ProjectActivity : BaseEntity
{

    public int ProjectID { get; set; }
    public int ActivityID { get; set; }
    public decimal RevenueRate { get; set; }
    public decimal SkilledLabourRate { get; set; }
    public decimal UnSkilledLabourRate { get; set; }
    public decimal OtherLabourRate { get; set; }
    public bool OutputRequired { get; set; }
    public bool Enabled { get; set; }
    public bool IsActive { get; set; }
}

