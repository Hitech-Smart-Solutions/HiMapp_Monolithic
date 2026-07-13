using MediatR;

namespace Himapp.Store.Application.Features.GatePasses.Queries;

public sealed record GetAllGatePassesQuery : IRequest<IReadOnlyCollection<GatePassDto>>;
