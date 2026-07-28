using Himapp.SharedKernel.Abstractions;

namespace Himapp.Safety.Domain.Clearance.Events;

public sealed record LabourClearanceChanged(int ProjectIdValue, int LabourId, bool InductionOk, bool TestsOk, bool MedicalOk)
    : DomainEvent(ProjectIdValue)
{
    public override string EventType => "Safety.LabourClearanceChanged";
}
