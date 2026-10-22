using Himapp.SharedKernel.Abstractions;

namespace Himapp.PM.Domain.Entities;

public sealed class EquipmentBreakdownReasons : BaseEntity
{
    public string BreakdownReason { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public int Type { get; set; }
}
