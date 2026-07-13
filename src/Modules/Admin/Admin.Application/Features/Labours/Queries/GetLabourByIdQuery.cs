using MediatR;

namespace Himapp.Admin.Application.Features.Labours.Queries;

public sealed record GetLabourByIdQuery(long Id) : IRequest<LabourDto?>;
