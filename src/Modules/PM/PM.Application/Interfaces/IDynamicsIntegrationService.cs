


using Himapp.PM.Domain.Entities;
using Himapp.Shared.Responses.PM;

namespace Himapp.PM.Application.Interfaces
{
    public interface IDynamicsIntegrationService 
    {
        Task<SyncResultDTO> SyncAssetTypes(bool isUpdateSync, int? logId = null);
        Task<List<DynamicsEquipmentCategory>> ResetAssetTypesInDynamics(int? logId = null);
        Task<List<DynamicsEquipmentMaster>> ResetAssetMasterIntegrationInDynamics(int? logId = null);
        Task<SyncResultDTO> SyncAssetMasterIntegration(bool isUpdateSync, int? logId = null);
    }
}