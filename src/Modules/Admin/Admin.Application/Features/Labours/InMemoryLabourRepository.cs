using System.Collections.Concurrent;
using System.Reflection;
using LabourEntity = Himapp.Admin.Domain.Labour.Labour;

namespace Himapp.Admin.Application.Features.Labours;

internal sealed class InMemoryLabourRepository : ILabourRepository
{
    private static readonly PropertyInfo IdProperty = typeof(LabourEntity).GetProperty(nameof(LabourEntity.Id))!;
    private readonly ConcurrentDictionary<long, LabourEntity> _labours = new();
    private long _nextId;

    public Task<IReadOnlyCollection<LabourEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        IReadOnlyCollection<LabourEntity> result = _labours.Values.OrderBy(labour => labour.Id).ToArray();
        return Task.FromResult(result);
    }

    public Task<LabourEntity?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _labours.TryGetValue(id, out var labour);
        return Task.FromResult(labour);
    }

    public Task AddAsync(LabourEntity labour, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var id = Interlocked.Increment(ref _nextId);
        IdProperty.SetValue(labour, id);
        _labours[id] = labour;
        return Task.CompletedTask;
    }

    public Task UpdateAsync(LabourEntity labour, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _labours[labour.Id] = labour;
        return Task.CompletedTask;
    }

    public Task<bool> DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(_labours.TryRemove(id, out _));
    }
}
