using Himapp.SharedKernel.Abstractions;

namespace Himapp.Execution.Domain.Entities;

// EF entity for schema table "Manpower" Page Name : Site Manpower Entry
public class Manpower : BaseEntity
{
    public Manpower()
    {
        ManpowerDetail = new HashSet<ManpowerDetail>();
    }
    public int ProjectID { get; set; }
    public int SectionID { get; set; }
    public DateOnly EntryDate { get; set; }
    public string? Remarks { get; set; }
    public int StateID { get; set; } = 3;
    public bool IsActive { get; set; }
    public virtual ICollection<ManpowerDetail>? ManpowerDetail { get; set; }
}

