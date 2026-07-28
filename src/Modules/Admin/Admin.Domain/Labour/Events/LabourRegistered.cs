using Himapp.SharedKernel.Abstractions;

namespace Himapp.Admin.Domain.Labour.Events;

public sealed record LabourRegistered(int ProjectIdValue, int LabourId, string LabourName, int ContractorId)
    : DomainEvent(ProjectIdValue)
{
    public override string EventType => "Admin.LabourRegistered";
}
