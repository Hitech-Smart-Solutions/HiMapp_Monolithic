using MediatR;

namespace Himapp.Store.Application.Features.GatePasses.Commands;

public sealed record UpdateGatePassCommand(
    long Id,
    long ProjectId,
    string GatePassNo,
    string Path,
    long? ServiceRequestId,
    string? BackdatedReason,
    IReadOnlyCollection<GatePassLineRequest> Lines,
    UploadedFileInfo? SupportingDocument) : IRequest<GatePassDto?>;
