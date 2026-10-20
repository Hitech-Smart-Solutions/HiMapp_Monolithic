

using Himapp.Shared.Responses.PM;
using Microsoft.Extensions.Logging;
using Himapp.Integrations.D365.Services;
using System.Net.Http.Headers;
using Himapp.PM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Himapp.PM.Domain.Constants.Enums;
using Himapp.PM.Application.DTOs;
namespace Himapp.PM.Application.Services;

public class DynamicsIntegrationService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<DynamicsIntegrationService> _logger;
    private readonly IPMDbContext _db;
    private ID365SyncService _d365SyncService;

    public DynamicsIntegrationService(HttpClient httpClient, ILogger<DynamicsIntegrationService> logger, IPMDbContext db)
    {
        _httpClient = httpClient;
        _logger = logger;
        _db = db;
        _d365SyncService = new NoopD365SyncService(null, null, null);
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

            var existingAssetTypes = await _db.DynamicsAssetTypes
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
                    _db.DynamicsAssetTypes.Add(new DynamicsAssetType
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


    public async Task<List<DynamicsAssetType>> ResetAssetTypesInDynamics()
    {
        var token = await _d365SyncService.GetDynamicsToken();

        List<DynamicsAssetType> dynamicsAssetType = await _db.DynamicsAssetTypes
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

    
}