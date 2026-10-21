using Himapp.SharedKernel.Abstractions;

namespace Himapp.PM.Domain.Entities;

public sealed class EquipmentSubPart : BaseEntity
{
    public string EquipmentSubPartName { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}
