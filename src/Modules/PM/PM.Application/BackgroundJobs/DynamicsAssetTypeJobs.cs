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

    // Kept without a parameter for any existing caller; the run is then logged as "System".
    public Task RunAddAssetTypeSync() => RunAddAssetTypeSync(null);

    /// <param name="logId">Id of the DynamicsSyncLog row created when the job was queued (null = create a new one).</param>
    public async Task RunAddAssetTypeSync(int? logId)
    {
        _logger.LogInformation(
            "Starting AssetType Add Sync background job. LogId: {LogId}", logId);

        var result =
            await _dynamicsIntegrationService
                .SyncAssetTypes(false, logId);

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

    // Kept without a parameter for any existing caller; the run is then logged as "System".
    public Task RunUpdateAssetTypeSync() => RunUpdateAssetTypeSync(null);

    /// <param name="logId">Id of the DynamicsSyncLog row created when the job was queued (null = create a new one).</param>
    public async Task RunUpdateAssetTypeSync(int? logId)
    {
        _logger.LogInformation(
            "Starting AssetType Update Sync background job. LogId: {LogId}", logId);

        var result =
            await _dynamicsIntegrationService
                .SyncAssetTypes(true, logId);

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

    // Kept without a parameter for any existing caller; the run is then logged as "System".
    public Task RunResetAssetTypeSync() => RunResetAssetTypeSync(null);

    /// <param name="logId">Id of the DynamicsSyncLog row created when the job was queued (null = create a new one).</param>
    public async Task RunResetAssetTypeSync(int? logId)
    {
        _logger.LogInformation(
            "Starting AssetType Reset background job. LogId: {LogId}", logId);

        await _dynamicsIntegrationService
            .ResetAssetTypesInDynamics(logId);

        _logger.LogInformation(
            "AssetType Reset Reset completed.");
    }

    // Kept without a parameter for any existing caller; the run is then logged as "System".
    public Task RunAddAssetMasterSync() => RunAddAssetMasterSync(null);

    /// <param name="logId">Id of the DynamicsSyncLog row created when the job was queued (null = create a new one).</param>
    public async Task RunAddAssetMasterSync(int? logId)
    {
        _logger.LogInformation(
            "Starting AssetMaster Add Sync background job. LogId: {LogId}", logId);

        var result =
            await _dynamicsIntegrationService
                .SyncAssetMasterIntegration(false, logId);

        if (!result.Success)
        {
            throw new InvalidOperationException(
                result.Message);
        }

        _logger.LogInformation(
            "AssetMaster Add Sync completed. " +
            "Inserted: {Inserted}, Updated: {Updated}",
            result.Inserted,
            result.Updated);
    }

    // Kept without a parameter for any existing caller; the run is then logged as "System".
    public Task RunUpdateAssetMasterSync() => RunUpdateAssetMasterSync(null);

    /// <param name="logId">Id of the DynamicsSyncLog row created when the job was queued (null = create a new one).</param>
    public async Task RunUpdateAssetMasterSync(int? logId)
    {
        _logger.LogInformation(
            "Starting AssetMaster Update Sync background job. LogId: {LogId}", logId);

        var result =
            await _dynamicsIntegrationService
                .SyncAssetMasterIntegration(true, logId);

        if (!result.Success)
        {
            throw new InvalidOperationException(
                result.Message);
        }

        _logger.LogInformation(
            "AssetMaster Update Sync completed. " +
            "Inserted: {Inserted}, Updated: {Updated}",
            result.Inserted,
            result.Updated);
    }

    // Kept without a parameter for any existing caller; the run is then logged as "System".
    public Task RunResetAssetMasterSync() => RunResetAssetMasterSync(null);

    /// <param name="logId">Id of the DynamicsSyncLog row created when the job was queued (null = create a new one).</param>
    public async Task RunResetAssetMasterSync(int? logId)
    {
        _logger.LogInformation(
            "Starting AssetMaster Reset background job. LogId: {LogId}", logId);

        await _dynamicsIntegrationService
            .ResetAssetMasterIntegrationInDynamics(logId);

        _logger.LogInformation(
            "AssetMaster Reset Reset completed.");
    }
}
