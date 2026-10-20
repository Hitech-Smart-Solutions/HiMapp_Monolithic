

using Himapp.PM.Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Himapp.PM.Application.Controllers;

[ApiController]
[Authorize]
[Route("v1/dynamics/integration")]
public sealed class DynamicsIntegrationController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IDynamicsIntegrationService _dynamicsIntegrationService;
    private readonly ILogger<DynamicsIntegrationController> _logger;

    public DynamicsIntegrationController(IMediator mediator, IDynamicsIntegrationService dynamicsIntegrationService, ILogger<DynamicsIntegrationController> logger)
    {
        _mediator = mediator;
        _dynamicsIntegrationService = dynamicsIntegrationService;
        _logger = logger;
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

}