using Himapp.SharedKernel.Abstractions;
using Himapp.Store.Domain.GatePass.Events;

namespace Himapp.Store.Domain.GatePass;

public sealed class GatePass : BaseEntity
{
    public int ProjectId { get; init; }
    public string GatePassNo { get; init; } = string.Empty;
    public string Path { get; init; } = "A";
    public int? ServiceRequestId { get; init; }
    public string Status { get; private set; } = "Draft";
    public string? BackdatedReason { get; init; }
    public string? CancelReason { get; private set; }
    public List<GatePassLine> Lines { get; } = [];

    public void Submit()
    {
        Status = "Submitted";
        Raise(new GatePassSubmitted(ProjectId, Id, GatePassNo));
    }

    public void Approve()
    {
        Status = "Approved";
        Raise(new GatePassApproved(ProjectId, Id, ServiceRequestId));
    }

    public void Cancel(string reason)
    {
        CancelReason = reason;
        Status = "Cancelled";
    }
}

public sealed record GatePassLine(string LineType, int? ItemId, int? EquipmentId, decimal Quantity, string Uom, DateOnly? ExpectedReturnDate, decimal ReturnedQuantity);
