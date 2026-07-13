using Himapp.Store.Application.Features.GatePasses.Commands;
using MediatR;

namespace Himapp.Store.Application.Features.GatePasses.Handlers;

internal sealed class CreateGatePassCommandHandler : IRequestHandler<CreateGatePassCommand, GatePassDto>
{
    private readonly IGatePassRepository _repository;

    public CreateGatePassCommandHandler(IGatePassRepository repository) => _repository = repository;

    public async Task<GatePassDto> Handle(CreateGatePassCommand request, CancellationToken cancellationToken)
    {
        var gatePass = new GatePassRecord(
            0,
            request.ProjectId,
            request.GatePassNo,
            request.Path,
            request.ServiceRequestId,
            "Draft",
            request.BackdatedReason,
            request.SupportingDocument,
            request.Lines.ToLines());

        var created = await _repository.AddAsync(gatePass, cancellationToken);
        return created.ToDto();
    }
}
