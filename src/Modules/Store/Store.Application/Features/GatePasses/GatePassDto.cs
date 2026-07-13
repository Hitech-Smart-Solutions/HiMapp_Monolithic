namespace Himapp.Store.Application.Features.GatePasses;

public sealed record GatePassDto(
    long Id,
    long ProjectId,
    string GatePassNo,
    string Path,
    long? ServiceRequestId,
    string Status,
    string? BackdatedReason,
    UploadedFileInfo? SupportingDocument,
    IReadOnlyCollection<GatePassLineDto> Lines);

public sealed record GatePassLineDto(
    string LineType,
    long? ItemId,
    long? EquipmentId,
    decimal Quantity,
    string Uom,
    DateOnly? ExpectedReturnDate,
    decimal ReturnedQuantity);

public sealed record GatePassLineRequest(
    string LineType,
    long? ItemId,
    long? EquipmentId,
    decimal Quantity,
    string Uom,
    DateOnly? ExpectedReturnDate,
    decimal ReturnedQuantity);

public sealed record UploadedFileInfo(string FileName, string ContentType, long SizeBytes);
