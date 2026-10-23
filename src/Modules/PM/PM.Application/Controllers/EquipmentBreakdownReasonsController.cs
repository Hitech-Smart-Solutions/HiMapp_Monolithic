using Himapp.PM.Application.Features;
using Himapp.PM.Application.Features.EquipmentBreakdownReasons.Commands;
using Himapp.PM.Application.Features.EquipmentBreakdownReasons.Models;
using Himapp.PM.Application.Features.EquipmentBreakdownReasons.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Himapp.PM.Application.Controllers;

[ApiController]
[Authorize]
[Route("v1/pm/equipment-breakdown-reasons")]
public sealed class EquipmentBreakdownReasonsController : ControllerBase
{
    private readonly IMediator _mediator;

    public EquipmentBreakdownReasonsController(IMediator mediator) => _mediator = mediator;

    /// <summary>Retrieves all equipment breakdown reasons.</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken) =>
        Ok(await _mediator.Send(new GetAllEquipmentBreakdownReasonsQuery(), cancellationToken));

    /// <summary>Retrieves the equipment category, manufacturer, and model lookup data.</summary>
    [HttpGet("category-manufacturer-models")]
    public async Task<IActionResult> GetCategoryManufacturerModels(CancellationToken cancellationToken) =>
        Ok(await _mediator.Send(new GetEquipmentCategoryManufacturerModelsQuery(), cancellationToken));

    /// <summary>Retrieves a paginated breakdown reason list with optional filtering, sorting, and active-status selection.</summary>
    [HttpGet("list")]
    public async Task<IActionResult> GetList(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? filterValue = null,
        [FromQuery] string? sortColumn = "ID ASC",
        [FromQuery] string? isActive = "all",
        CancellationToken cancellationToken = default) =>
        Ok(await _mediator.Send(
            new GetEquipmentBreakdownReasonsList(page, pageSize, filterValue, sortColumn, isActive),
            cancellationToken));

    /// <summary>Retrieves the equipment breakdown reason with the specified identifier.</summary>
    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetEquipmentBreakdownReasonsByIdQuery(id), cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    /// <summary>Creates an equipment breakdown reason from the supplied request.</summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] EquipmentBreakdownReasonsRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.BreakdownReason))
            return BadRequest("Equipment breakdown reason is required.");

        if (!Enum.IsDefined(request.Type))
            return BadRequest("Equipment breakdown reason type must be Hydraulic, Electrical, or Mechanical.");

        var result = await _mediator.Send(
            new CreateEquipmentBreakdownReasonsCommand(
                request.BreakdownReason,
                request.IsActive,
                request.Type,
                request.CreateBy,
                request.LastModifiedBy),
            cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>Updates the equipment breakdown reason with the specified identifier using the supplied request.</summary>
    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(int id, [FromBody] EquipmentBreakdownReasonsRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.BreakdownReason))
            return BadRequest("Equipment breakdown reason is required.");

        if (!Enum.IsDefined(request.Type))
            return BadRequest("Equipment breakdown reason type must be Hydraulic, Electrical, or Mechanical.");

        var result = await _mediator.Send(
            new UpdateEquipmentBreakdownReasonsCommand(
                id,
                request.BreakdownReason,
                request.IsActive,
                request.Type,
                request.LastModifiedBy),
            cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    /// <summary>Changes the active status of the breakdown reason identified by the supplied action history.</summary>
    [HttpPut("SetActiveInActiveBreakdownReasons")]
    public async Task<IActionResult> Delete([FromBody] AddTransactionActionHistoryDTO dtoInactive, CancellationToken cancellationToken)
    {
        if (dtoInactive is null)
            return BadRequest("Delete request is required.");

        if (dtoInactive.ProgramRowId <= 0)
            return BadRequest("Equipment breakdown reason ID is required.");

        var deleted = await _mediator.Send(new DeleteEquipmentBreakdownReasonsCommand(dtoInactive), cancellationToken);
        return deleted ? Ok() : NotFound();
    }
}
