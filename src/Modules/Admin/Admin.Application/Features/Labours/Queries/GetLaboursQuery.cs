using MediatR;

namespace Himapp.Admin.Application.Features.Labours.Queries;

public sealed record GetLaboursQuery : IRequest<IReadOnlyCollection<LabourDto>>;
