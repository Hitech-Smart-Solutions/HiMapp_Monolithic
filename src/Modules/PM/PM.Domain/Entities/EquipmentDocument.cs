using Himapp.SharedKernel.Abstractions;

namespace Himapp.PM.Domain.Entities;

public sealed class EquipmentDocument : BaseEntity
{
    public int DocumentTypeID { get; set; }
    public string DocumentName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool? IsActive { get; set; } = true;
}
