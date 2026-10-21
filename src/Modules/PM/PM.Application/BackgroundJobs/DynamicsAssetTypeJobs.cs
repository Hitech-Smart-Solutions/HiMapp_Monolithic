using Himapp.PM.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace Himapp.PM.Application.BackgroundJobs;

public class DynamicsAssetTypeJobs
{
    private readonly IDynamicsIntegrationService
        _dynamicsIntegrationService;

    private readonly ILogger<DynamicsAssetTypeJobs>
        _logger;

    public DynamicsAssetTypeJobs(
        IDynamicsIntegrationService dynamicsIntegrationService,
        ILogger<DynamicsAssetTypeJobs> logger)
    {
        _dynamicsIntegrationService =
            dynamicsIntegrationService;

        _logger = logger;
    }

    public async Task RunAddAssetTypeSync()
    {
        _logger.LogInformation(
            "Starting AssetType Add Sync background job.");

        var result =
            await _dynamicsIntegrationService
                .SyncAssetTypes(false);

        if (!result.Success)
        {
            throw new InvalidOperationException(
                result.Message);
        }

        _logger.LogInformation(
            "AssetType Add Sync completed. " +
            "Inserted: {Inserted}, Updated: {Updated}",
            result.Inserted,
            result.Updated);
    }


    public async Task RunUpdateAssetTypeSync()
    {
        _logger.LogInformation(
            "Starting AssetType Update Sync background job.");

        var result =
            await _dynamicsIntegrationService
                .SyncAssetTypes(true);

        if (!result.Success)
        {
            throw new InvalidOperationException(
                result.Message);
        }

        _logger.LogInformation(
            "AssetType Update Sync completed. " +
            "Inserted: {Inserted}, Updated: {Updated}",
            result.Inserted,
            result.Updated);
    }


    public async Task RunResetAssetTypeSync()
    {
        _logger.LogInformation(
            "Starting AssetType Reset background job.");

        await _dynamicsIntegrationService
            .ResetAssetTypesInDynamics();

        _logger.LogInformation(
            "AssetType Reset Sync completed.");
    }
}