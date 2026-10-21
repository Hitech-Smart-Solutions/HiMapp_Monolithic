using Himapp.SharedKernel.Abstractions;

namespace Himapp.PM.Domain.Entities;

public sealed class SpecificationMaster : BaseEntity
{
    public int Type { get; set; }
    public string SpecificationName { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}