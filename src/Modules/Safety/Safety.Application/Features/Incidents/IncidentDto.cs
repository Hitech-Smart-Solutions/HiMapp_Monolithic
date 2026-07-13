namespace Himapp.Safety.Application.Features.Incidents;

public sealed record IncidentDto(
    long Id,
    long ProjectId,
    string Title,
    string Severity,
    DateOnly OccurredOn,
    string Description,
    string Status,
    UploadedFileInfo? Attachment);

public sealed record UploadedFileInfo(string FileName, string ContentType, long SizeBytes);
