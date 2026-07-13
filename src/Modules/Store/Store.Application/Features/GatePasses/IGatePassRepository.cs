namespace Himapp.Store.Application.Features.GatePasses;

internal interface IGatePassRepository
{
    Task<IReadOnlyCollection<GatePassRecord>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<GatePassRecord?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<GatePassRecord> AddAsync(GatePassRecord gatePass, CancellationToken cancellationToken = default);
    Task<GatePassRecord?> UpdateAsync(GatePassRecord gatePass, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

internal sealed record GatePassRecord(
    long Id,
    long ProjectId,
    string GatePassNo,
    string Path,
    long? ServiceRequestId,
    string Status,
    string? BackdatedReason,
    UploadedFileInfo? SupportingDocument,
    IReadOnlyCollection<GatePassLineDto> Lines);
