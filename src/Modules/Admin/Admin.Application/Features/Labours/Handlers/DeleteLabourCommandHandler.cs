using Himapp.Admin.Application.Features.Labours.Commands;
using MediatR;

namespace Himapp.Admin.Application.Features.Labours.Handlers;

internal sealed class DeleteLabourCommandHandler : IRequestHandler<DeleteLabourCommand, bool>
{
    private readonly ILabourRepository _repository;

    public DeleteLabourCommandHandler(ILabourRepository repository) => _repository = repository;

    public Task<bool> Handle(DeleteLabourCommand request, CancellationToken cancellationToken) =>
        _repository.DeleteAsync(request.Id, cancellationToken);
}
