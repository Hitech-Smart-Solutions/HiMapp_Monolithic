

using Himapp.Shared.Responses.PM;
using Microsoft.Extensions.Logging;
using Himapp.Integrations.D365.Services;
using System.Net.Http.Headers;
using Himapp.PM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Himapp.PM.Domain.Constants.Enums;
using Himapp.PM.Application.DTOs;
using Himapp.PM.Application.Interfaces;
using Himapp.PM.Domain.Constants;
namespace Himapp.PM.Application.Services;

public class DynamicsIntegrationService : IDynamicsIntegrationService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<DynamicsIntegrationService> _logger;
    private readonly IPMDbContext _db;
    private readonly ID365SyncService _d365SyncService;
    private readonly IDynamicsSyncLogService _syncLog;

    public DynamicsIntegrationService(HttpClient httpClient, ILogger<DynamicsIntegrationService> logger, IPMDbContext db, ID365SyncService d365SyncService, IDynamicsSyncLogService syncLog)
    {
        _httpClient = httpClient;
        _logger = logger;
        _db = db;
        _d365SyncService = d365SyncService;
        _syncLog = syncLog;
    }
    
    #region Sync AssetTypes

    public async Task<SyncResultDTO> SyncAssetTypes(bool isUpdateSync, int? logId = null)
    {
        var run = await _syncLog.BeginAsync(DynamicsSyncEntity.AssetType, isUpdateSync ? DynamicsSyncMode.Update : DynamicsSyncMode.Add, logId);
        if (run is null)
            return new SyncResultDTO { Success = true, Message = "This run was already completed." };

        SyncResultDTO result;
        try
        {
            result = await SyncAssetTypesCore(isUpdateSync, run.UserId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error syncing asset types");
            result = new SyncResultDTO { Success = false, Message = $"Error occurred: {ex.Message}" };
        }

        await _syncLog.CompleteAsync(run.LogId, result);
        return result;
    }

    private async Task<SyncResultDTO> SyncAssetTypesCore(bool isUpdateSync, int userId)
    {
        try
        {
            var token = await _d365SyncService.GetDynamicsToken();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            string updatedValue = isUpdateSync ? "Yes" : "No";
            var filter = $"ISPLIsUpdated eq Microsoft.Dynamics.DataEntities.NoYes'{updatedValue}' " +
                            "and ISPLIsSync eq Microsoft.Dynamics.DataEntities.NoYes'No'";

            var dynamicsAssetTypes = await _d365SyncService.FetchFromDynamics<DynamicsAssetTypeDTO>("ISPLAssetObjectTypes", filter);
            //var dynamicsAssetTypes = await FetchAssetTypesFromDynamics(filter);

            if (!dynamicsAssetTypes.Any())
            {
                return new SyncResultDTO { Success = true, Message = "No asset types to sync." };
            }

            var existingAssetTypes = await _db.DynamicsEquipmentCategory
                .ToDictionaryAsync(x => x.OBJECTTYPEID);

            int inserted = 0;
            int updated = 0;

            var now = _d365SyncService.NormalizeDynamicsDate(DateTime.UtcNow);

            foreach (var at in dynamicsAssetTypes)
            {
                var fuelRequired = at.ISFuelConsumptionRequired_ISPL == "Yes" ? ISPLYesNo.Yes : ISPLYesNo.No;
                var logbookRequired = at.IsLogbookRequired_ISPL == "Yes" ? ISPLYesNo.Yes : ISPLYesNo.No;

                if (!existingAssetTypes.TryGetValue(at.ObjectTypeID, out var existing))
                {
                    _db.DynamicsEquipmentCategory.Add(new DynamicsEquipmentCategory
                    {
                        UniqueID = Guid.NewGuid(),
                        OBJECTTYPEID = at.ObjectTypeID,
                        DataAreaId = at.DataAreaId,
                        OBJECTLIFECYCLEMODELID = at.ObjectLifecycleModelId,
                        NAME = at.Name,
                        SHORTNAME_ISPL = at.ShortName_ISPL,
                        ISFUELCONSUMPTIONREQUIRED_ISPL = fuelRequired,
                        ISLOGBOOKREQUIRED_ISPL = logbookRequired,
                        ISPLISUPDATED = ISPLISUpdated.No,
                        ISPLISSYNC = ISPLISSync.Yes,
                        CreatedBy = userId,
                        CreatedDate = now,
                        LastModifiedBy = userId,
                        LastModifiedDate = now
                    });
                    inserted++;
                }
                else
                {
                    existing.OBJECTLIFECYCLEMODELID = at.ObjectLifecycleModelId;
                    existing.NAME = at.Name;
                    existing.SHORTNAME_ISPL = at.ShortName_ISPL;
                    existing.ISFUELCONSUMPTIONREQUIRED_ISPL = fuelRequired;
                    existing.ISLOGBOOKREQUIRED_ISPL = logbookRequired;
                    existing.ISPLISUPDATED = ISPLISUpdated.No;
                    existing.ISPLISSYNC = ISPLISSync.Yes;
                    existing.LastModifiedBy = userId;
                    existing.LastModifiedDate = now;
                    updated++;
                }
            }

            await _db.SaveChangesAsync();

            var payload = new { ISPLIsSync = "Yes", ISPLIsUpdated = "No" };
            foreach (var at in dynamicsAssetTypes)
            {
                var resourcePath = $"ISPLAssetObjectTypes(dataAreaId='{at.DataAreaId}',ObjectTypeID='{at.ObjectTypeID}')";
                await _d365SyncService.PatchToDynamics(resourcePath, token, payload);
                //await MarkAssetTypeAsSyncedInDynamics(at.DataAreaId, at.ObjectTypeID, token, payload);
            }

            return new SyncResultDTO
            {
                Success = true,
                Fetched = dynamicsAssetTypes.Count(),
                Inserted = inserted,
                Updated = updated,
                Message = $"AssetType sync completed. Inserted: {inserted}, Updated: {updated}"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error syncing asset types");
            return new SyncResultDTO { Success = false, Message = $"Error occurred: {ex.Message}" };
        }
    }


    public async Task<List<DynamicsEquipmentCategory>> ResetAssetTypesInDynamics(int? logId = null)
    {
        var run = await _syncLog.BeginAsync(DynamicsSyncEntity.AssetType, DynamicsSyncMode.Reset, logId);
        if (run is null)
            return new List<DynamicsEquipmentCategory>();

        try
        {
            var reset = await ResetAssetTypesInDynamicsCore();
            await _syncLog.CompleteAsync(run.LogId, DynamicsSyncStatus.Success, reset.Count, $"Asset type reset completed. Records reset: {reset.Count}");
            return reset;
        }
        catch (Exception ex)
        {
            await _syncLog.CompleteAsync(run.LogId, DynamicsSyncStatus.Failed, 0, $"Error occurred: {ex.Message}", ex.ToString());
            throw;
        }
    }

    private async Task<List<DynamicsEquipmentCategory>> ResetAssetTypesInDynamicsCore()
    {
        var token = await _d365SyncService.GetDynamicsToken();

        List<DynamicsEquipmentCategory> dynamicsAssetType = await _db.DynamicsEquipmentCategory
            .OrderBy(x => x.OBJECTTYPEID).ToListAsync();

        var payload = new
        {
            ISPLIsSync = "No",
            ISPLIsUpdated = "No"
        };

        foreach (var at in dynamicsAssetType)
        {
            var resourcePath = $"ISPLAssetObjectTypes(dataAreaId='{at.DataAreaId}',ObjectTypeID='{at.OBJECTTYPEID}')";
            await  _d365SyncService.PatchToDynamics(resourcePath, token, payload);
            //await MarkAssetTypeAsSyncedInDynamics(site.DataAreaId, site.OBJECTTYPEID, token, payload);
        }

        return dynamicsAssetType;
    }

    #endregion

    
        #region Sync Asset Integration

        public async Task<SyncResultDTO> SyncAssetMasterIntegration(bool isUpdateSync, int? logId = null)
        {
            var run = await _syncLog.BeginAsync(DynamicsSyncEntity.AssetMaster, isUpdateSync ? DynamicsSyncMode.Update : DynamicsSyncMode.Add, logId);
            if (run is null)
                return new SyncResultDTO { Success = true, Message = "This run was already completed." };

            SyncResultDTO result;
            try
            {
                result = await SyncAssetMasterIntegrationCore(isUpdateSync, run.UserId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error syncing asset master integration");
                result = new SyncResultDTO { Success = false, Message = $"Error occurred: {ex.Message}" };
            }

            await _syncLog.CompleteAsync(run.LogId, result);
            return result;
        }

        private async Task<SyncResultDTO> SyncAssetMasterIntegrationCore(bool isUpdateSync, int userId)
        {
            try
            {
                var token = await _d365SyncService.GetDynamicsToken();
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                string updatedValue = isUpdateSync ? "Yes" : "No";
                var filter = $"ISPLIsUpdated eq Microsoft.Dynamics.DataEntities.NoYes'{updatedValue}' " +
                             "and ISPLIsSync eq Microsoft.Dynamics.DataEntities.NoYes'No'";

                var dynamicsAssetMasters = await  _d365SyncService.FetchFromDynamics<DynamicsAssetMasterIntegrationDTO>("ISPLAssetObjectTables", filter);
                //var dynamicsAssetMasters = await _d365SyncService.FetchAssetMasterIntegrationFromDynamics(filter);

                if (!dynamicsAssetMasters.Any())
                {
                    return new SyncResultDTO { Success = true, Message = "No asset masters to sync." };
                }

                var existingAssets = await _db.DynamicsEquipmentMaster
                    .ToDictionaryAsync(x => x.OBJECTID);

                int inserted = 0;
                int updated = 0;

                var now = _d365SyncService.NormalizeDynamicsDate(DateTime.UtcNow);

                foreach (var dto in dynamicsAssetMasters)
                {
                    var isHired = dto.ISPLISHIREDASSET == "Yes" ? ISPLYesNo.Yes : ISPLYesNo.No;
                    var isActive = dto.OBJECTACTIVE == "Yes" ? ISPLYesNo.Yes : ISPLYesNo.No;

                    if (!existingAssets.TryGetValue(dto.OBJECTID, out var entity))
                    {
                        _db.DynamicsEquipmentMaster.Add(new DynamicsEquipmentMaster
                        {
                            UniqueID = Guid.NewGuid(),
                            CreatedBy = userId,
                            CreatedDate = (DateTime)now,
                            LastModifiedBy = userId,
                            LastModifiedDate = (DateTime)now,
                            OBJECTID = dto.OBJECTID,
                            DataAreaId = dto.DataAreaId,
                            ACQUISITIONCOST = dto.ACQUISITIONCOST,
                            ACQUISITIONDATE = dto.ACQUISITIONDATE,
                            ACTIVEFROM = dto.ACTIVEFROM,
                            ACTIVETO = dto.ACTIVETO,
                            CATEGORYLEVEL1 = dto.CATEGORYLEVEL1,
                            CATEGORYLEVEL2 = dto.CATEGORYLEVEL2,
                            CATEGORYLEVEL3 = dto.CATEGORYLEVEL3,
                            CRITICALITY = dto.CRITICALITY,
                            FIXEDASSETID = dto.FIXEDASSETID,
                            FIXEDASSETNAME = dto.FIXEDASSETNAME,
                            FUNCTIONALLOCATION = dto.FUNCTIONALLOCATION,
                            ISPLBATCHNUMBER = dto.ISPLBATCHNUMBER,
                            ISPLHIREDDATE = dto.ISPLHIREDDATE,
                            ISPLISHIREDASSET = isHired,
                            OBJECTACTIVE = isActive,
                            ISPLITEMID = dto.ISPLITEMID,
                            ISPLITEMNAME = dto.ISPLITEMNAME,
                            ISPLPONUMBER = dto.ISPLPONUMBER,
                            ISPLVEHICLENUMBER = dto.ISPLVEHICLENUMBER,
                            ISPLVENDORACCOUNT = dto.ISPLVENDORACCOUNT,
                            MANUFACTURER = dto.MANUFACTURER,
                            MODEL = dto.MODEL,
                            MODELRECID = dto.MODELRECID,
                            MODELYEAR = dto.MODELYEAR,
                            NAME = dto.NAME,
                            OBJECTIDPARENT = dto.OBJECTIDPARENT,
                            OBJECTIDPARENTNAME = dto.OBJECTIDPARENTNAME,
                            OBJECTLIFECYCLESTATE = dto.OBJECTLIFECYCLESTATE,
                            OBJECTTYPE = dto.OBJECTTYPE,
                            PURCHID = dto.PURCHID,
                            RESOURCE = dto.RESOURCE,
                            SERIALID = dto.SERIALID,
                            SERVICELEVEL = dto.SERVICELEVEL,
                            STDFUELCONSUMPTION_ISPL = dto.STDFUELCONSUMPTION_ISPL,
                            VENDACCOUNT = dto.VENDACCOUNT,
                            VENDNAME = dto.VENDNAME,
                            WARRANTYDATEFROMVEND =  _d365SyncService.NormalizeDynamicsDate(dto.WARRANTYDATEFROMVEND),
                            WARRANTYDATETOVEND = _d365SyncService.NormalizeDynamicsDate(dto.WARRANTYDATETOVEND),
                            WARRANTYVEND = dto.WARRANTYVEND,
                            WRKCTRLDESCRIPTION = dto.WRKCTRLDESCRIPTION,
                            ISPLISUPDATED = ISPLISUpdated.No,
                            ISPLISSYNC = ISPLISSync.Yes
                        });

                        inserted++;
                    }
                    else
                    {
                        entity.ACQUISITIONCOST = dto.ACQUISITIONCOST;
                        entity.ACQUISITIONDATE = dto.ACQUISITIONDATE;
                        entity.ACTIVEFROM = dto.ACTIVEFROM;
                        entity.ACTIVETO = dto.ACTIVETO;
                        entity.CATEGORYLEVEL1 = dto.CATEGORYLEVEL1;
                        entity.CATEGORYLEVEL2 = dto.CATEGORYLEVEL2;
                        entity.CATEGORYLEVEL3 = dto.CATEGORYLEVEL3;
                        entity.CRITICALITY = dto.CRITICALITY;
                        entity.FIXEDASSETID = dto.FIXEDASSETID;
                        entity.FIXEDASSETNAME = dto.FIXEDASSETNAME;
                        entity.FUNCTIONALLOCATION = dto.FUNCTIONALLOCATION;
                        entity.WARRANTYDATEFROMVEND = _d365SyncService.NormalizeDynamicsDate(dto.WARRANTYDATEFROMVEND);
                        entity.WARRANTYDATETOVEND = _d365SyncService.NormalizeDynamicsDate(dto.WARRANTYDATETOVEND);
                        entity.ISPLBATCHNUMBER = dto.ISPLBATCHNUMBER;
                        entity.ISPLHIREDDATE = dto.ISPLHIREDDATE;
                        entity.ISPLISHIREDASSET = isHired;
                        entity.OBJECTACTIVE = isActive;
                        entity.NAME = dto.NAME;
                        entity.ISPLISUPDATED = ISPLISUpdated.No;
                        entity.ISPLISSYNC = ISPLISSync.Yes;
                        entity.LastModifiedBy = userId;
                        entity.LastModifiedDate = (DateTime)now;
                        updated++;
                    }
                }

                await _db.SaveChangesAsync();

                var payload = new { ISPLIsSync = "Yes", ISPLIsUpdated = "No" };
                foreach (var dto in dynamicsAssetMasters)
                {
                    var resourcePath = $"ISPLAssetObjectTables(dataAreaId='{dto.DataAreaId}',ObjectID='{dto.OBJECTID}')";
                    await _d365SyncService.PatchToDynamics(resourcePath, token, payload);
                    //await MarkAssetMasterIntegrationAsSyncedInDynamics(dto.DataAreaId, dto.OBJECTID, token, payload);
                }

                return new SyncResultDTO
                {
                    Success = true,
                    Fetched = dynamicsAssetMasters.Count(),
                    Inserted = inserted,
                    Updated = updated,
                    Message = $"Asset master sync completed. Inserted: {inserted}, Updated: {updated}"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error syncing asset master integration");
                return new SyncResultDTO { Success = false, Message = $"Error occurred: {ex.Message}" };
            }
        }

        public async Task<List<DynamicsEquipmentMaster>> ResetAssetMasterIntegrationInDynamics(int? logId = null)
        {
            var run = await _syncLog.BeginAsync(DynamicsSyncEntity.AssetMaster, DynamicsSyncMode.Reset, logId);
            if (run is null)
                return new List<DynamicsEquipmentMaster>();

            try
            {
                var reset = await ResetAssetMasterIntegrationInDynamicsCore();
                await _syncLog.CompleteAsync(run.LogId, DynamicsSyncStatus.Success, reset.Count, $"Asset master reset completed. Records reset: {reset.Count}");
                return reset;
            }
            catch (Exception ex)
            {
                await _syncLog.CompleteAsync(run.LogId, DynamicsSyncStatus.Failed, 0, $"Error occurred: {ex.Message}", ex.ToString());
                throw;
            }
        }

        private async Task<List<DynamicsEquipmentMaster>> ResetAssetMasterIntegrationInDynamicsCore()
        {
            var token = await _d365SyncService.GetDynamicsToken();

            var dynamicsAssetMasterIntegration = await _db.DynamicsEquipmentMaster
                .OrderBy(x => x.OBJECTID)
                .ToListAsync();

            var payload = new
            {
                ISPLIsSync = "No",
                ISPLIsUpdated = "No"
            };

            foreach (var site in dynamicsAssetMasterIntegration)
            {
                var resourcePath = $"ISPLAssetObjectTables(dataAreaId='{site.DataAreaId}',ObjectID='{site.OBJECTID}')";
                await _d365SyncService.PatchToDynamics(resourcePath, token, payload);
                //await MarkAssetMasterIntegrationAsSyncedInDynamics(site.DataAreaId, site.OBJECTID, token, payload);
            }

            return dynamicsAssetMasterIntegration;
        }


    #endregion
}