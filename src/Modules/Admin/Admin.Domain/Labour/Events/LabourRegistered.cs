using Himapp.SharedKernel.Abstractions;

namespace Himapp.Admin.Domain.Labour.Events;

public sealed record LabourRegistered(long ProjectIdValue, long LabourId, string LabourName, long ContractorId)
    : DomainEvent(ProjectIdValue)
{
    public override string EventType => "Admin.LabourRegistered";
}
