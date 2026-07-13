using Himapp.Store.Application.Features.GatePasses.Queries;
using MediatR;

namespace Himapp.Store.Application.Features.GatePasses.Handlers;

internal sealed class GetAllGatePassesQueryHandler : IRequestHandler<GetAllGatePassesQuery, IReadOnlyCollection<GatePassDto>>
{
    private readonly IGatePassRepository _repository;

    public GetAllGatePassesQueryHandler(IGatePassRepository repository) => _repository = repository;

    public async Task<IReadOnlyCollection<GatePassDto>> Handle(GetAllGatePassesQuery request, CancellationToken cancellationToken)
    {
        var gatePasses = await _repository.GetAllAsync(cancellationToken);
        return gatePasses.Select(gatePass => gatePass.ToDto()).ToArray();
    }
}
