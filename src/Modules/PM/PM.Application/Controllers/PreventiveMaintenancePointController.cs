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

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken) =>
        Ok(await _mediator.Send(new GetAllPreventiveMaintenancePointsQuery(), cancellationToken));

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetPreventiveMaintenancePointByIdQuery(id), cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpGet("lookup")]
    public async Task<IActionResult> Lookup(CancellationToken ct) =>
    Ok(await _mediator.Send(new GetPreventiveMaintenancePointLookupQuery(), ct));

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

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
    {
        var deleted = await _mediator.Send(new DeletePreventiveMaintenancePointCommand(id), cancellationToken);
        return deleted ? Ok() : NotFound();
    }
}