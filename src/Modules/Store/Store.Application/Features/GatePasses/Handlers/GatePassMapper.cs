namespace Himapp.Store.Application.Features.GatePasses.Handlers;

internal static class GatePassMapper
{
    public static GatePassDto ToDto(this GatePassRecord gatePass) =>
        new(
            gatePass.Id,
            gatePass.ProjectId,
            gatePass.GatePassNo,
            gatePass.Path,
            gatePass.ServiceRequestId,
            gatePass.Status,
            gatePass.BackdatedReason,
            gatePass.SupportingDocument,
            gatePass.Lines);

    public static IReadOnlyCollection<GatePassLineDto> ToLines(this IReadOnlyCollection<GatePassLineRequest> lines) =>
        lines.Select(line => new GatePassLineDto(
                line.LineType,
                line.ItemId,
                line.EquipmentId,
                line.Quantity,
                line.Uom,
                line.ExpectedReturnDate,
                line.ReturnedQuantity))
            .ToArray();
}
