using Himapp.PM.Application.Features.PreventiveMaintenancePoints.Commands;
using Himapp.PM.Application.Features.PreventiveMaintenancePoints.Models;
using Himapp.PM.Application.Features.PreventiveMaintenancePoints.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Himapp.PM.Application.Controllers;

[ApiController]
[Authorize]
[Route("v1/plant-machinery/preventive-maintenance-point")]
public sealed class PreventiveMaintenancePointController : ControllerBase
{
    private readonly IMediator _mediator;
    public PreventiveMaintenancePointController(IMediator mediator) => _mediator = mediator;

    /// <summary>Retrieves all preventive maintenance points.</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken) =>
        Ok(await _mediator.Send(new GetAllPreventiveMaintenancePointsQuery(), cancellationToken));

    /// <summary>Retrieves a paginated preventive maintenance point list with optional filtering, sorting, and active-status selection.</summary>
    [HttpGet("list")]
    public async Task<IActionResult> GetList(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? filterColumn = null,
        [FromQuery] string? filterValue = null,
        [FromQuery] string? sortColumn = "ID ASC",
        [FromQuery] string? isActive = "all",
        CancellationToken cancellationToken = default) =>
        Ok(await _mediator.Send(
            new GetPreventiveMaintenancePointList(page, pageSize, filterColumn, filterValue, sortColumn, isActive),
            cancellationToken));

    /// <summary>Retrieves the preventive maintenance point with the specified identifier.</summary>
    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetPreventiveMaintenancePointByIdQuery(id), cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    /// <summary>Retrieves the preventive maintenance point lookup data.</summary>
    [HttpGet("lookup")]
    public async Task<IActionResult> Lookup(CancellationToken ct) =>
    Ok(await _mediator.Send(new GetPreventiveMaintenancePointLookupQuery(), ct));

    /// <summary>Creates a preventive maintenance point from the supplied request.</summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] PreventiveMaintenancePointRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.MaintenancePointName))
        {
            return BadRequest("Maintenance point name is required.");
        }

        try
        {
            var result = await _mediator.Send(
                new CreatePreventiveMaintenancePointCommand(request.MaintenancePointName.Trim(), request.StatusID, request.CreateBy, request.LastModifiedBy),
                cancellationToken);

            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    /// <summary>Updates the preventive maintenance point with the specified identifier using the supplied request.</summary>
    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, [FromBody] PreventiveMaintenancePointRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.MaintenancePointName))
        {
            return BadRequest("Maintenance point name is required.");
        }

        try
        {
            var result = await _mediator.Send(
                new UpdatePreventiveMaintenancePointCommand(id, request.MaintenancePointName.Trim(), request.StatusID, request.LastModifiedBy),
                cancellationToken);

            return result is null ? NotFound() : Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    /// <summary>Changes the active status of the preventive maintenance point identified by the supplied action history.</summary>
    [HttpPut("SetActiveInActivePreventiveMaintenancePoints")]
    public async Task<IActionResult> Delete([FromBody] AddTransactionActionHistoryDTO dtoInactive, CancellationToken cancellationToken)
    {
        if (dtoInactive is null)
            return BadRequest("Delete request is required.");

        if (dtoInactive.ProgramRowId <= 0)
            return BadRequest("Preventive maintenance point ID is required.");

        var deleted = await _mediator.Send(new DeletePreventiveMaintenancePointCommand(dtoInactive), cancellationToken);
        return deleted ? Ok() : NotFound();
    }
}
