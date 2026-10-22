using Himapp.PM.Application.Features.Common;
using Himapp.PM.Application.Features.DynamicsEquipmentMasters.Commands;
using Himapp.PM.Application.Features.DynamicsEquipmentMasters.Models;
using Himapp.PM.Application.Features.DynamicsEquipmentMasters.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Himapp.PM.Application.Controllers;

[ApiController]
[Authorize]
[Route("v1/pm/dynamics-equipment-masters")]
public sealed class DynamicsEquipmentMastersController : ControllerBase
{
    private readonly IMediator _mediator;
    public DynamicsEquipmentMastersController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] ListSearchParams searchParams, CancellationToken cancellationToken)
    {
        try
        {
            return Ok(await _mediator.Send(new GetDynamicsEquipmentMastersQuery(searchParams), cancellationToken));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetDynamicsEquipmentMasterByIdQuery(id), cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] DynamicsEquipmentMasterRequest request, CancellationToken cancellationToken)
    {
        var error = Validate(request);
        if (error is not null) return BadRequest(new { message = error });

        try
        {
            var result = await _mediator.Send(new CreateDynamicsEquipmentMasterCommand(request), cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] DynamicsEquipmentMasterRequest request, CancellationToken cancellationToken)
    {
        var error = Validate(request);
        if (error is not null) return BadRequest(new { message = error });

        try
        {
            var result = await _mediator.Send(new UpdateDynamicsEquipmentMasterCommand(id, request), cancellationToken);
            return result is null ? NotFound() : Ok(result);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    /// <summary>Delete (soft delete to be enabled once IsActive is migrated).</summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        try
        {
            var deleted = await _mediator.Send(new DeleteDynamicsEquipmentMasterCommand(id), cancellationToken);
            return deleted ? Ok() : NotFound();
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
    }

    private static string? Validate(DynamicsEquipmentMasterRequest r)
    {
        if (string.IsNullOrWhiteSpace(r.DataAreaId)) return "DataAreaId is required.";
        if (string.IsNullOrWhiteSpace(r.OBJECTID)) return "OBJECTID is required.";
        if (string.IsNullOrWhiteSpace(r.NAME)) return "NAME is required.";
        return null;
    }
}
