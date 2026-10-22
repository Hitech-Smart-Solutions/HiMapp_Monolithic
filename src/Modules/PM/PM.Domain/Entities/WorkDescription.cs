using Himapp.SharedKernel.Abstractions;

namespace Himapp.PM.Domain.Entities;

public sealed class WorkDescription : BaseEntity
{
    public string WorkDescriptionName { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}
