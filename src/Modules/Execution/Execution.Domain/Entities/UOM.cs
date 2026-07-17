namespace Himapp.Execution.Domain.Entities;

// EF entity for schema table "UOM"
public sealed class UOM
{
    public long Id { get; set; }
    public Guid UniqueId { get; set; }

    public long CompanyId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;

    public bool IsActive { get; set; }

    public long? CreatedBy { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
    public long? LastModifiedBy { get; set; }
    public DateTimeOffset LastModifiedDate { get; set; }
}
