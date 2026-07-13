using MediatR;

namespace Himapp.Store.Application.Features.GatePasses.Commands;

public sealed record DeleteGatePassCommand(long Id) : IRequest<bool>;
