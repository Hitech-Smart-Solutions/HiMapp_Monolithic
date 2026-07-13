using System.Collections.Concurrent;

namespace Himapp.PM.Application.Features.Equipments;

internal sealed class InMemoryEquipmentRepository : IEquipmentRepository
{
    private readonly ConcurrentDictionary<long, EquipmentDto> _equipments = new();
    private long _nextId;

    public Task<IReadOnlyCollection<EquipmentDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        IReadOnlyCollection<EquipmentDto> result = _equipments.Values.OrderBy(equipment => equipment.Id).ToArray();
        return Task.FromResult(result);
    }

    public Task<EquipmentDto?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _equipments.TryGetValue(id, out var equipment);
        return Task.FromResult(equipment);
    }

    public Task<EquipmentDto> AddAsync(EquipmentDto equipment, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var created = equipment with { Id = Interlocked.Increment(ref _nextId), Status = "Available" };
        _equipments[created.Id] = created;
        return Task.FromResult(created);
    }

    public Task<EquipmentDto?> UpdateAsync(EquipmentDto equipment, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!_equipments.ContainsKey(equipment.Id))
        {
            return Task.FromResult<EquipmentDto?>(null);
        }

        _equipments[equipment.Id] = equipment;
        return Task.FromResult<EquipmentDto?>(equipment);
    }

    public Task<bool> DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(_equipments.TryRemove(id, out _));
    }
}
