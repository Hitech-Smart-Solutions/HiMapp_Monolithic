

using Himapp.PM.Application.BackgroundJobs;
using Himapp.PM.Application.Interfaces;
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

    public DynamicsIntegrationController(IMediator mediator, IDynamicsIntegrationService dynamicsIntegrationService, ILogger<DynamicsIntegrationController> logger, IBackgroundJobService backgroundJobService)
    {
        _mediator = mediator;
        _dynamicsIntegrationService = dynamicsIntegrationService;
        _logger = logger;
        _backgroundJobService = backgroundJobService;
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

        [HttpPost("run-job/assettype-add")]
        public IActionResult RunAddAssetTypeSync()
        {
            var jobId =
                _backgroundJobService
                    .Enqueue<DynamicsAssetTypeJobs>(
                        job => job.RunAddAssetTypeSync());

            return Accepted(new
            {
                JobId = jobId,
                Message = "AssetType add sync has been queued."
            });
        }

        [HttpPost("run-job/assettype-update")]
        public IActionResult RunUpdateAssetTypeSync()
        {
            var jobId =
                _backgroundJobService
                    .Enqueue<DynamicsAssetTypeJobs>(
                        job => job.RunUpdateAssetTypeSync());

            return Accepted(new
            {
                JobId = jobId,
                Message = "AssetType update sync has been queued."
            });
        }

        [HttpPost("run-job/assettype-reset")]
        public IActionResult RunResetAssetTypeSync()
        {
            var jobId =
                _backgroundJobService
                    .Enqueue<DynamicsAssetTypeJobs>(
                        job => job.RunResetAssetTypeSync());

            return Accepted(new
            {
                JobId = jobId,
                Message = "AssetType reset sync has been queued."
            });
        }



}