namespace Himapp.Execution.Domain.Entities;

// EF entity for schema table "DailyLaborDetails"
public sealed class DailyLaborDetail
{
    public long Id { get; set; }
    public Guid UniqueId { get; set; }

    public long DailyLaborId { get; set; }
    public long ContractorId { get; set; }
    public long ActivityId { get; set; }

    public int SkilledCount { get; set; }
    public int UnskilledCount { get; set; }
    public int MatCount { get; set; }

    // computed stored column in DB
    public int TotalCount { get; set; }

    public bool IsActive { get; set; }

    public long? CreatedBy { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
    public long? LastModifiedBy { get; set; }
    public DateTimeOffset LastModifiedDate { get; set; }
}

