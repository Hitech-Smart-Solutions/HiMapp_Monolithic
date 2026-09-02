using Himapp.SharedKernel.Abstractions;
using System.Text.Json.Serialization;

namespace Himapp.Execution.Domain.Entities;

// EF entity for schema table "DailyLaborDetails"  Page Name : Project Manpower Entry
public class DailyLaborDetail : BaseEntity
{
    public int DailyLabourID { get; set; }
    public int? ContractorID { get; set; }
    public bool IsActive { get; set; } = true;
    public int? CategoryID { get; set; }
    public int? Skilled { get; set; }
    public int? UnSkilled { get; set; }
    public string? Remarks { get; set; }
    public int? Mat { get; set; }
    public string? ContractorName { get; set; }
    public int? ActivityID { get; set; }
    [JsonIgnore]
    public virtual DailyLabor? DailyLabor { get; set; }
}

