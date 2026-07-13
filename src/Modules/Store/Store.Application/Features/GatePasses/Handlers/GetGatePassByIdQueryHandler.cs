using Himapp.Store.Application.Features.GatePasses.Queries;
using MediatR;

namespace Himapp.Store.Application.Features.GatePasses.Handlers;

internal sealed class GetGatePassByIdQueryHandler : IRequestHandler<GetGatePassByIdQuery, GatePassDto?>
{
    private readonly IGatePassRepository _repository;

    public GetGatePassByIdQueryHandler(IGatePassRepository repository) => _repository = repository;

    public async Task<GatePassDto?> Handle(GetGatePassByIdQuery request, CancellationToken cancellationToken)
    {
        var gatePass = await _repository.GetByIdAsync(request.Id, cancellationToken);
        return gatePass?.ToDto();
    }
}
