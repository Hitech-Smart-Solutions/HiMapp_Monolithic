using Himapp.PM.Application.Features.EquipmentSubParts.Commands;
using Himapp.PM.Application.Features.EquipmentSubParts.Models;
using Himapp.PM.Application.Features.EquipmentSubParts.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Himapp.PM.Application.Controllers;

[ApiController]
[Authorize]
[Route("v1/pm/equipment-sub-parts")]
public sealed class EquipmentSubPartsController : ControllerBase
{
    private readonly IMediator _mediator;

    public EquipmentSubPartsController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken) =>
        Ok(await _mediator.Send(new GetAllEquipmentSubPartsQuery(), cancellationToken));

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
            new GetEquipmentSubPartList(page, pageSize, filterColumn, filterValue, sortColumn, isActive),
            cancellationToken));

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetEquipmentSubPartByIdQuery(id), cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] EquipmentSubPartRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.EquipmentSubPartName))
            return BadRequest("Equipment sub-part name is required.");

        var result = await _mediator.Send(
            new CreateEquipmentSubPartCommand(request.EquipmentSubPartName, request.IsActive, request.CreateBy, request.LastModifiedBy),
            cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(int id, [FromBody] EquipmentSubPartRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.EquipmentSubPartName))
            return BadRequest("Equipment sub-part name is required.");

        var result = await _mediator.Send(
            new UpdateEquipmentSubPartCommand(id, request.EquipmentSubPartName, request.IsActive, request.LastModifiedBy),
            cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await _mediator.Send(new DeleteEquipmentSubPartCommand(id), cancellationToken);
        return deleted ? Ok() : NotFound();
    }
}
