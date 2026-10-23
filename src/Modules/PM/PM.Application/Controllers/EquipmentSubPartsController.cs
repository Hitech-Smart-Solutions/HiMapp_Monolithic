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

    /// <summary>Retrieves all equipment sub-parts.</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken) =>
        Ok(await _mediator.Send(new GetAllEquipmentSubPartsQuery(), cancellationToken));

    /// <summary>Retrieves a paginated equipment sub-part list with optional filtering, sorting, and active-status selection.</summary>
    [HttpGet("list")]
    public async Task<IActionResult> GetList(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? filterValue = null,
        [FromQuery] string? sortColumn = "ID ASC",
        [FromQuery] string? isActive = "all",
        CancellationToken cancellationToken = default) =>
        Ok(await _mediator.Send(
            new GetEquipmentSubPartList(page, pageSize, filterValue, sortColumn, isActive),
            cancellationToken));

    /// <summary>Retrieves the equipment sub-part with the specified identifier.</summary>
    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetEquipmentSubPartByIdQuery(id), cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    /// <summary>Creates an equipment sub-part from the supplied request.</summary>
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

    /// <summary>Updates the equipment sub-part with the specified identifier using the supplied request.</summary>
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

    /// <summary>Changes the active status of the equipment sub-part identified by the supplied action history.</summary>
    [HttpPut("SetActiveInActiveEquipmentSubParts")]
    public async Task<IActionResult> Delete([FromBody] AddTransactionActionHistoryDTO dtoInactive, CancellationToken cancellationToken)
    {
        if (dtoInactive is null)
            return BadRequest("Delete request is required.");

        if (dtoInactive.ProgramRowId <= 0)
            return BadRequest("Equipment sub-part ID is required.");

        var deleted = await _mediator.Send(new DeleteEquipmentSubPartCommand(dtoInactive), cancellationToken);
        return deleted ? Ok() : NotFound();
    }
}
