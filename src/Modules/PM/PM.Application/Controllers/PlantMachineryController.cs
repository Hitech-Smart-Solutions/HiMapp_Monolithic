using Himapp.PM.Application.Features.Equipments;
using Himapp.PM.Application.Features.Equipments.Commands;
using Himapp.PM.Application.Features.Equipments.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Himapp.PM.Application.Controllers;

[ApiController]
[Authorize]
[Route("v1/plant-machinery/equipments")]
public sealed class PlantMachineryController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IEquipmentPreventiveMaintenancePointService _preventiveMaintenancePointService;

    public PlantMachineryController(
        IMediator mediator,
        IEquipmentPreventiveMaintenancePointService preventiveMaintenancePointService)
    {
        _mediator = mediator;
        _preventiveMaintenancePointService = preventiveMaintenancePointService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken) =>
        Ok(await _mediator.Send(new GetAllEquipmentsQuery(), cancellationToken));

    [HttpGet("awaiting-pm-configuration")]
    public async Task<IActionResult> GetAwaitingPMConfigurationQueue(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 25,
        [FromQuery] int? categoryId = null,
        [FromQuery] string? search = null,
        CancellationToken cancellationToken = default)
    {
        if (page < 1 || pageSize < 1)
        {
            return BadRequest("Page must be at least 1 and page size must be positive.");
        }

        return Ok(await _mediator.Send(
            new GetAwaitingPMConfigurationQueueQuery(page, pageSize, categoryId, search),
            cancellationToken));
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetEquipmentByIdQuery(id), cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] EquipmentRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.AssetCode))
        {
            return BadRequest("Asset code is required.");
        }

        var result = await _mediator.Send(
            new CreateEquipmentCommand(request.ProjectId, request.AssetCode, request.Name, request.Category, request.MaintenanceDueOn),
            cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, [FromBody] EquipmentRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new UpdateEquipmentCommand(id, request.ProjectId, request.AssetCode, request.Name, request.Category, request.MaintenanceDueOn),
            cancellationToken);

        return result is null ? NotFound() : Ok(result);
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
    {
        var deleted = await _mediator.Send(new DeleteEquipmentCommand(id), cancellationToken);
        return deleted ? Ok() : NotFound();
    }

    [HttpGet("{equipmentId:int}/preventive-maintenance-points")]
    public async Task<IActionResult> GetPreventiveMaintenancePoints(int equipmentId, CancellationToken cancellationToken) =>
        Ok(await _preventiveMaintenancePointService.GetByEquipmentAsync(equipmentId, cancellationToken));

    [HttpGet("{equipmentId:int}/preventive-maintenance-points/{id:long}")]
    public async Task<IActionResult> GetPreventiveMaintenancePoint(int equipmentId, long id, CancellationToken cancellationToken)
    {
        var result = await _preventiveMaintenancePointService.GetByIdAsync(equipmentId, id, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost("{equipmentId:int}/preventive-maintenance-points")]
    public async Task<IActionResult> CreatePreventiveMaintenancePoint(
        int equipmentId,
        [FromBody] EquipmentPMConfigurationRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _preventiveMaintenancePointService.CreateAsync(equipmentId, request, cancellationToken);
            return CreatedAtAction(nameof(GetPreventiveMaintenancePoint), new { equipmentId, id = result.Id }, result);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPut("{equipmentId:int}/preventive-maintenance-points/{id:long}")]
    public async Task<IActionResult> UpdatePreventiveMaintenancePoint(
        int equipmentId,
        long id,
        [FromBody] EquipmentPMConfigurationRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _preventiveMaintenancePointService.UpdateAsync(equipmentId, id, request, cancellationToken);
            return result is null ? NotFound() : Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpDelete("{equipmentId:int}/preventive-maintenance-points/{id:long}")]
    public async Task<IActionResult> DeactivatePreventiveMaintenancePoint(
        int equipmentId,
        long id,
        [FromQuery] int? lastModifiedBy,
        CancellationToken cancellationToken)
    {
        var deactivated = await _preventiveMaintenancePointService.DeactivateAsync(
            equipmentId,
            id,
            lastModifiedBy,
            cancellationToken);

        return deactivated ? Ok() : NotFound();
    }

}
