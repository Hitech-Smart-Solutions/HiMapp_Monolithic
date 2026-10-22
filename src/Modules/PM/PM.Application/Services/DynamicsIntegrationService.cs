

using Himapp.Shared.Responses.PM;
using Microsoft.Extensions.Logging;
using Himapp.Integrations.D365.Services;
using System.Net.Http.Headers;
using Himapp.PM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Himapp.PM.Domain.Constants.Enums;
using Himapp.PM.Application.DTOs;
using Himapp.PM.Application.Interfaces;
namespace Himapp.PM.Application.Services;

public class DynamicsIntegrationService : IDynamicsIntegrationService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<DynamicsIntegrationService> _logger;
    private readonly IPMDbContext _db;
    private readonly ID365SyncService _d365SyncService;

    public DynamicsIntegrationService(HttpClient httpClient, ILogger<DynamicsIntegrationService> logger, IPMDbContext db, ID365SyncService d365SyncService)
    {
        _httpClient = httpClient;
        _logger = logger;
        _db = db;
        _d365SyncService = d365SyncService;
    }
    
    #region Sync AssetTypes

    public async Task<SyncResultDTO> SyncAssetTypes(bool isUpdateSync)
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
                        CreatedDate = now,
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


    public async Task<List<DynamicsEquipmentCategory>> ResetAssetTypesInDynamics()
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

        public async Task<SyncResultDTO> SyncAssetMasterIntegration(bool isUpdateSync)
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

        public async Task<List<DynamicsEquipmentMaster>> ResetAssetMasterIntegrationInDynamics()
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