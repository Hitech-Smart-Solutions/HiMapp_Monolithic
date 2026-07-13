using System.Collections.Concurrent;

namespace Himapp.Execution.Application.Features.Activities;

internal sealed class InMemoryActivityRepository : IActivityRepository
{
    private readonly ConcurrentDictionary<long, ActivityDto> _activities = new();
    private long _nextId;

    public Task<IReadOnlyCollection<ActivityDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        IReadOnlyCollection<ActivityDto> result = _activities.Values.OrderBy(activity => activity.Id).ToArray();
        return Task.FromResult(result);
    }

    public Task<ActivityDto?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _activities.TryGetValue(id, out var activity);
        return Task.FromResult(activity);
    }

    public Task<ActivityDto> AddAsync(ActivityDto activity, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var created = activity with { Id = Interlocked.Increment(ref _nextId) };
        _activities[created.Id] = created;
        return Task.FromResult(created);
    }

    public Task<ActivityDto?> UpdateAsync(ActivityDto activity, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!_activities.ContainsKey(activity.Id))
        {
            return Task.FromResult<ActivityDto?>(null);
        }

        _activities[activity.Id] = activity;
        return Task.FromResult<ActivityDto?>(activity);
    }

    public Task<bool> DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(_activities.TryRemove(id, out _));
    }
}
