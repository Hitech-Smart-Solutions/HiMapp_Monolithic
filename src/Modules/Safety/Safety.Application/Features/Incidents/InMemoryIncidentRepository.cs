using System.Collections.Concurrent;

namespace Himapp.Safety.Application.Features.Incidents;

internal sealed class InMemoryIncidentRepository : IIncidentRepository
{
    private readonly ConcurrentDictionary<long, IncidentDto> _incidents = new();
    private long _nextId;

    public Task<IReadOnlyCollection<IncidentDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        IReadOnlyCollection<IncidentDto> result = _incidents.Values.OrderBy(incident => incident.Id).ToArray();
        return Task.FromResult(result);
    }

    public Task<IncidentDto?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _incidents.TryGetValue(id, out var incident);
        return Task.FromResult(incident);
    }

    public Task<IncidentDto> AddAsync(IncidentDto incident, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var created = incident with { Id = Interlocked.Increment(ref _nextId), Status = "Open" };
        _incidents[created.Id] = created;
        return Task.FromResult(created);
    }

    public Task<IncidentDto?> UpdateAsync(IncidentDto incident, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!_incidents.TryGetValue(incident.Id, out var existing))
        {
            return Task.FromResult<IncidentDto?>(null);
        }

        var updated = incident with { Attachment = incident.Attachment ?? existing.Attachment };
        _incidents[incident.Id] = updated;
        return Task.FromResult<IncidentDto?>(updated);
    }

    public Task<bool> DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(_incidents.TryRemove(id, out _));
    }
}
