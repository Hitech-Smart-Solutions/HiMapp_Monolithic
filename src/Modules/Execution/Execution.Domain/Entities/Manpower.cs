namespace Himapp.Execution.Domain.Entities;

// EF entity for schema table "Manpower" Page Name : Site Manpower Entry
public class Manpower
{
    public Manpower()
    {
        ManpowerDetail = new HashSet<ManpowerDetail>();
    }
    public int ID { get; set; }
    public Guid UniqueID { get; set; }
    public int ProjectID { get; set; }
    public int SectionID { get; set; }
    public DateOnly EntryDate { get; set; }
    public string? Remarks { get; set; }
    public int StateID { get; set; } = 3;
    public bool IsActive { get; set; }
    public int CreatedBy { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
    public int LastModifiedBy { get; set; }
    public DateTimeOffset LastModifiedDate { get; set; }
    public virtual ICollection<ManpowerDetail>? ManpowerDetail { get; set; }
}

