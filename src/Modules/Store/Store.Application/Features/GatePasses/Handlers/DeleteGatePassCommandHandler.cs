using Himapp.Store.Application.Features.GatePasses.Commands;
using Himapp.Store.Application.Infrastructure;
using MediatR;

namespace Himapp.Store.Application.Features.GatePasses.Handlers;

internal sealed class DeleteGatePassCommandHandler : IRequestHandler<DeleteGatePassCommand, bool>
{
    private readonly IBackgroundTaskQueue _backgroundTaskQueue;
    private readonly IGatePassRepository _repository;

    public DeleteGatePassCommandHandler(IGatePassRepository repository, IBackgroundTaskQueue backgroundTaskQueue)
    {
        _repository = repository;
        _backgroundTaskQueue = backgroundTaskQueue;
    }

    public async Task<bool> Handle(DeleteGatePassCommand request, CancellationToken cancellationToken)
    {
        var deleted = await _repository.DeleteAsync(request.Id, cancellationToken);
        if (deleted)
        {
            await _backgroundTaskQueue.QueueAsync(_ => ValueTask.CompletedTask, cancellationToken);
        }

        return deleted;
    }
}
