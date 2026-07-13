using MediatR;

namespace Himapp.Store.Application.Features.GatePasses.Queries;

public sealed record GetGatePassByIdQuery(long Id) : IRequest<GatePassDto?>;
