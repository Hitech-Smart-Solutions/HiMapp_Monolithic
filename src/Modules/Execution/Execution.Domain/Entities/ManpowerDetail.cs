using System.Text.Json.Serialization;

namespace Himapp.Execution.Domain.Entities;

// EF entity for schema table "ManpowerDetails"
public class ManpowerDetail
{
    public int ID { get; set; }
    public Guid UniqueID { get; set; }
    public int ManpowerID { get; set; }
    public int ContractorID { get; set; }
    public int ActivityID { get; set; }
    public int SkilledCount { get; set; }
    public int UnskilledCount { get; set; }
    public int OtherCount { get; set; }
    // computed stored column in DB
    public int TotalCount { get; set; }
    public bool IsActive { get; set; }
    public int CreatedBy { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
    public int LastModifiedBy { get; set; }
    public DateTimeOffset LastModifiedDate { get; set; }
    [JsonIgnore]
    public virtual Manpower? Manpower { get; set; }
}

