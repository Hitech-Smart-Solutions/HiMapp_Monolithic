


using Himapp.PM.Domain.Entities;
using Himapp.Shared.Responses.PM;

namespace Himapp.PM.Application.Interfaces
{
    public interface IDynamicsIntegrationService 
    {
        Task<SyncResultDTO> SyncAssetTypes(bool isUpdateSync);
        Task<List<DynamicsEquipmentCategory>> ResetAssetTypesInDynamics();
    }
}