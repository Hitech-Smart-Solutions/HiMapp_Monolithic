using Himapp.PM.Application.Features.Equipments;

namespace Himapp.PM.Application.Features.Equipments;

public interface IEquipmentPreventiveMaintenancePointService
{
    Task<IReadOnlyList<EquipmentPMConfigurationResponse>> GetByEquipmentAsync(int equipmentId, CancellationToken cancellationToken = default);
    Task<EquipmentPMConfigurationResponse?> GetByIdAsync(int equipmentId, long id, CancellationToken cancellationToken = default);
    Task<EquipmentPMConfigurationResponse> CreateAsync(int equipmentId, EquipmentPMConfigurationRequest request, CancellationToken cancellationToken = default);
    Task<EquipmentPMConfigurationResponse?> UpdateAsync(int equipmentId, long id, EquipmentPMConfigurationRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeactivateAsync(int equipmentId, long id, int? lastModifiedBy, CancellationToken cancellationToken = default);
}
