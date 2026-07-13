using MediatR;

namespace Himapp.Safety.Application.Features.Incidents.Commands;

public sealed record CreateIncidentCommand(long ProjectId, string Title, string Severity, DateOnly OccurredOn, string Description, UploadedFileInfo? Attachment) : IRequest<IncidentDto>;
public sealed record UpdateIncidentCommand(long Id, long ProjectId, string Title, string Severity, DateOnly OccurredOn, string Description, UploadedFileInfo? Attachment) : IRequest<IncidentDto?>;
public sealed record DeleteIncidentCommand(long Id) : IRequest<bool>;
