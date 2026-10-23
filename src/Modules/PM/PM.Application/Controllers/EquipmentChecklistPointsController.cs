using Himapp.PM.Application.Features;
using Himapp.PM.Application.Features.EquipmentChecklistPoints.Commands;
using Himapp.PM.Application.Features.EquipmentChecklistPoints.Models;
using Himapp.PM.Application.Features.EquipmentChecklistPoints.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Himapp.PM.Application.Controllers;

[ApiController]
[Authorize]
[Route("v1/pm/equipment-checklist-points")]
public sealed class EquipmentChecklistPointsController : ControllerBase
{
    private readonly IMediator _mediator;

    public EquipmentChecklistPointsController(IMediator mediator) => _mediator = mediator;

    /// <summary>Retrieves all equipment checklist points.</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken) =>
        Ok(await _mediator.Send(new GetAllEquipmentChecklistPointsQuery(), cancellationToken));

    /// <summary>Retrieves a paginated checklist point list with optional filtering, sorting, and active-status selection.</summary>
    [HttpGet("list")]
    public async Task<IActionResult> GetList(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? filterValue = null,
        [FromQuery] string? sortColumn = "ID ASC",
        [FromQuery] string? isActive = "all",
        CancellationToken cancellationToken = default) =>
        Ok(await _mediator.Send(
            new GetEquipmentChecklistPointList(page, pageSize, filterValue, sortColumn, isActive),
            cancellationToken));

    /// <summary>Retrieves the equipment checklist point with the specified identifier.</summary>
    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetEquipmentChecklistPointByIdQuery(id), cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    /// <summary>Creates an equipment checklist point from the supplied request.</summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] EquipmentChecklistPointRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Description))
            return BadRequest("Equipment checklist point description is required.");

        if (!Enum.IsDefined(request.Type))
            return BadRequest("Equipment checklist point type must be Daily or ECR.");

        var result = await _mediator.Send(
            new CreateEquipmentChecklistPointCommand(
                request.Description,
                request.IsActive,
                request.Type,
                request.CreateBy,
                request.LastModifiedBy),
            cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>Updates the equipment checklist point with the specified identifier using the supplied request.</summary>
    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(int id, [FromBody] EquipmentChecklistPointRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Description))
            return BadRequest("Equipment checklist point description is required.");

        if (!Enum.IsDefined(request.Type))
            return BadRequest("Equipment checklist point type must be Daily or ECR.");

        var result = await _mediator.Send(
            new UpdateEquipmentChecklistPointCommand(
                id,
                request.Description,
                request.IsActive,
                request.Type,
                request.LastModifiedBy),
            cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    /// <summary>Changes the active status of the checklist point identified by the supplied action history.</summary>
    [HttpPut("SetActiveInActiveChecklistPoints")]
    public async Task<IActionResult> Delete([FromBody] AddTransactionActionHistoryDTO dtoInactive, CancellationToken cancellationToken)
    {
        if (dtoInactive is null)
            return BadRequest("Delete request is required.");

        if (dtoInactive.ProgramRowId <= 0)
            return BadRequest("Equipment checklist point ID is required.");

        var deleted = await _mediator.Send(new DeleteEquipmentChecklistPointCommand(dtoInactive), cancellationToken);
        return deleted ? Ok() : NotFound();
    }
}
