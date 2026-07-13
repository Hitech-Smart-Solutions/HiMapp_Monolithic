using Himapp.SharedKernel.Abstractions;

namespace Himapp.Safety.Domain.Clearance.Events;

public sealed record LabourClearanceChanged(long ProjectIdValue, long LabourId, bool InductionOk, bool TestsOk, bool MedicalOk)
    : DomainEvent(ProjectIdValue)
{
    public override string EventType => "Safety.LabourClearanceChanged";
}
