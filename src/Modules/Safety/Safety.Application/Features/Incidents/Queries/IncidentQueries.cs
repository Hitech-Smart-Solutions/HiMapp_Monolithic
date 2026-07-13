using MediatR;

namespace Himapp.Safety.Application.Features.Incidents.Queries;

public sealed record GetAllIncidentsQuery : IRequest<IReadOnlyCollection<IncidentDto>>;
public sealed record GetIncidentByIdQuery(long Id) : IRequest<IncidentDto?>;
