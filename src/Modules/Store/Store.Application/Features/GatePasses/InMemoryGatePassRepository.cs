using System.Collections.Concurrent;

namespace Himapp.Store.Application.Features.GatePasses;

internal sealed class InMemoryGatePassRepository : IGatePassRepository
{
    private readonly ConcurrentDictionary<long, GatePassRecord> _gatePasses = new();
    private long _nextId;

    public Task<IReadOnlyCollection<GatePassRecord>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        IReadOnlyCollection<GatePassRecord> result = _gatePasses.Values.OrderBy(gatePass => gatePass.Id).ToArray();
        return Task.FromResult(result);
    }

    public Task<GatePassRecord?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _gatePasses.TryGetValue(id, out var gatePass);
        return Task.FromResult(gatePass);
    }

    public Task<GatePassRecord> AddAsync(GatePassRecord gatePass, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var id = Interlocked.Increment(ref _nextId);
        var created = gatePass with { Id = id, Status = "Submitted" };
        _gatePasses[id] = created;
        return Task.FromResult(created);
    }

    public Task<GatePassRecord?> UpdateAsync(GatePassRecord gatePass, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!_gatePasses.ContainsKey(gatePass.Id))
        {
            return Task.FromResult<GatePassRecord?>(null);
        }

        _gatePasses[gatePass.Id] = gatePass;
        return Task.FromResult<GatePassRecord?>(gatePass);
    }

    public Task<bool> DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(_gatePasses.TryRemove(id, out _));
    }
}
