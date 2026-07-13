using Himapp.Store.Application.Features.GatePasses.Commands;
using MediatR;

namespace Himapp.Store.Application.Features.GatePasses.Handlers;

internal sealed class UpdateGatePassCommandHandler : IRequestHandler<UpdateGatePassCommand, GatePassDto?>
{
    private readonly IGatePassRepository _repository;

    public UpdateGatePassCommandHandler(IGatePassRepository repository) => _repository = repository;

    public async Task<GatePassDto?> Handle(UpdateGatePassCommand request, CancellationToken cancellationToken)
    {
        var existing = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (existing is null)
        {
            return null;
        }

        var gatePass = existing with
        {
            ProjectId = request.ProjectId,
            GatePassNo = request.GatePassNo,
            Path = request.Path,
            ServiceRequestId = request.ServiceRequestId,
            BackdatedReason = request.BackdatedReason,
            SupportingDocument = request.SupportingDocument ?? existing.SupportingDocument,
            Lines = request.Lines.ToLines()
        };

        var updated = await _repository.UpdateAsync(gatePass, cancellationToken);
        return updated?.ToDto();
    }
}
