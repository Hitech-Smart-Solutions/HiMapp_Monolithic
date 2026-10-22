using Himapp.PM.Application.BackgroundJobs;
using Himapp.PM.Application.Features.Common;
using Himapp.PM.Application.Features.DynamicsSyncLogs.Queries;
using Himapp.PM.Application.Interfaces;
using Himapp.PM.Application.Services;
using Himapp.PM.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Shared.Integrations.D365.BackgroundJobs.Interfaces;

namespace Himapp.PM.Application.Controllers;

[ApiController]
[Authorize]
[Route("v1/dynamics/integration")]
public sealed class DynamicsIntegrationController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IDynamicsIntegrationService _dynamicsIntegrationService;
    private readonly ILogger<DynamicsIntegrationController> _logger;
    private readonly IBackgroundJobService _backgroundJobService;
    private readonly IDynamicsSyncLogService _syncLog;

    public DynamicsIntegrationController(IMediator mediator, IDynamicsIntegrationService dynamicsIntegrationService, ILogger<DynamicsIntegrationController> logger, IBackgroundJobService backgroundJobService, IDynamicsSyncLogService syncLog)
    {
        _mediator = mediator;
        _dynamicsIntegrationService = dynamicsIntegrationService;
        _logger = logger;
        _backgroundJobService = backgroundJobService;
        _syncLog = syncLog;
    }

    #region AssetType

    /// <summary>
    /// Executes Sync Dynamics Asset Type.
    /// </summary>
    [HttpPost("SyncDynamicsAssetType")]
    public async Task<IActionResult> SyncDynamicsAssetType()
    {
        try
        {
            bool isUpdateSync = false;
            var result = await _dynamicsIntegrationService.SyncAssetTypes(isUpdateSync);
            return Ok(result);
        }
        catch (SyncAlreadyRunningException ex)
        {
            return Conflict(new { success = false, message = ex.Message });
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Asset Types sync failed");

            return BadRequest(new
            {
                success = false,
                message = "Failed to sync Asset Type",
                error = ex.Message
            });
        }
    }

    /// <summary>
    /// Updates Update Sync Asset Types.
    /// </summary>
    [HttpGet("UpdateDynamicsAssetTypes")]
    public async Task<IActionResult> UpdateSyncAssetTypes()
    {
        try
        {
            bool isUpdateSync = true;
            var sites = await _dynamicsIntegrationService.SyncAssetTypes(isUpdateSync);
            return Ok(sites);
        }
        catch (SyncAlreadyRunningException ex)
        {
            return Conflict(new { success = false, message = ex.Message });
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled error in UpdateSyncAssetTypes controller");
            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                success = false,
                message = "Failed to fetch updated Asset Type."
            });
        }
    }

    /// <summary>
    /// Executes Reset Sync Asset Types.
    /// </summary>
    [HttpGet("ResetDynamicsAssetTypes")]
    public async Task<IActionResult> ResetSyncAssetTypes()
    {
        try
        {
            var sites = await _dynamicsIntegrationService.ResetAssetTypesInDynamics();
            return Ok(sites);
        }
        catch (SyncAlreadyRunningException ex)
        {
            return Conflict(new { success = false, message = ex.Message });
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled error in ResetSyncAssetTypes controller");

            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                success = false,
                message = "Failed to reset Asset Type in dynamics."
            });
        }
    }

    #endregion

    #region Background jobs

    [HttpPost("run-job/assettype-add")]
    public Task<IActionResult> RunAddAssetTypeSync() =>
        QueueJobAsync(DynamicsSyncEntity.AssetType, DynamicsSyncMode.Add, "AssetType add sync",
            logId => _backgroundJobService.Enqueue<DynamicsAssetTypeJobs>(job => job.RunAddAssetTypeSync(logId)));

    [HttpPost("run-job/assettype-update")]
    public Task<IActionResult> RunUpdateAssetTypeSync() =>
        QueueJobAsync(DynamicsSyncEntity.AssetType, DynamicsSyncMode.Update, "AssetType update sync",
            logId => _backgroundJobService.Enqueue<DynamicsAssetTypeJobs>(job => job.RunUpdateAssetTypeSync(logId)));

    [HttpPost("run-job/assettype-reset")]
    public Task<IActionResult> RunResetAssetTypeSync() =>
        QueueJobAsync(DynamicsSyncEntity.AssetType, DynamicsSyncMode.Reset, "AssetType reset sync",
            logId => _backgroundJobService.Enqueue<DynamicsAssetTypeJobs>(job => job.RunResetAssetTypeSync(logId)));

    [HttpPost("run-job/assetmaster-add")]
    public Task<IActionResult> RunAddAssetMasterSync() =>
        QueueJobAsync(DynamicsSyncEntity.AssetMaster, DynamicsSyncMode.Add, "AssetMaster add sync",
            logId => _backgroundJobService.Enqueue<DynamicsAssetTypeJobs>(job => job.RunAddAssetMasterSync(logId)));

    [HttpPost("run-job/assetmaster-update")]
    public Task<IActionResult> RunUpdateAssetMasterSync() =>
        QueueJobAsync(DynamicsSyncEntity.AssetMaster, DynamicsSyncMode.Update, "AssetMaster update sync",
            logId => _backgroundJobService.Enqueue<DynamicsAssetTypeJobs>(job => job.RunUpdateAssetMasterSync(logId)));

    [HttpPost("run-job/assetmaster-reset")]
    public Task<IActionResult> RunResetAssetMasterSync() =>
        QueueJobAsync(DynamicsSyncEntity.AssetMaster, DynamicsSyncMode.Reset, "AssetMaster reset sync",
            logId => _backgroundJobService.Enqueue<DynamicsAssetTypeJobs>(job => job.RunResetAssetMasterSync(logId)));

    /// <summary>
    /// Creates the log row (Queued) first, so a second user is refused straight away while a run is queued or running,
    /// then hands the job to the background service.
    /// </summary>
    private async Task<IActionResult> QueueJobAsync(string entityName, string syncMode, string description, Func<int, string> enqueue)
    {
        try
        {
            var logId = await _syncLog.QueueAsync(entityName, syncMode);

            string jobId;
            try
            {
                jobId = enqueue(logId);
            }
            catch (Exception ex)
            {
                await _syncLog.CompleteAsync(logId, DynamicsSyncStatus.Failed, 0, $"Could not queue the job: {ex.Message}", ex.ToString());
                throw;
            }

            await _syncLog.SetJobIdAsync(logId, jobId);

            return Accepted(new
            {
                JobId = jobId,
                LogId = logId,
                Message = $"{description} has been queued."
            });
        }
        catch (SyncAlreadyRunningException ex)
        {
            return Conflict(new { success = false, message = ex.Message });
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
    }

    #endregion

    #region Sync log

    /// <summary>
    /// Lists the sync / reset runs (newest first unless a sort column is given).
    /// Filter on Status (Queued, Running, Success, Failed, NothingToSync) to see what is currently running.
    /// </summary>
    [HttpGet("sync-logs")]
    public async Task<IActionResult> GetSyncLogs([FromQuery] ListSearchParams searchParams, CancellationToken cancellationToken)
    {
        try
        {
            return Ok(await _mediator.Send(new GetDynamicsSyncLogsQuery(searchParams), cancellationToken));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    #endregion
}
