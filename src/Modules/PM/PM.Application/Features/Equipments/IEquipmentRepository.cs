namespace Himapp.PM.Application.Features.Equipments;

internal interface IEquipmentRepository
{
    Task<IReadOnlyCollection<EquipmentDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<EquipmentDto?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<EquipmentDto> AddAsync(EquipmentDto equipment, CancellationToken cancellationToken = default);
    Task<EquipmentDto?> UpdateAsync(EquipmentDto equipment, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(long id, CancellationToken cancellationToken = default);
}
