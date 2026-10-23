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

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken) =>
        Ok(await _mediator.Send(new GetAllEquipmentBreakdownReasonsQuery(), cancellationToken));

    [HttpGet("category-manufacturer-models")]
    public async Task<IActionResult> GetCategoryManufacturerModels(CancellationToken cancellationToken) =>
        Ok(await _mediator.Send(new GetEquipmentCategoryManufacturerModelsQuery(), cancellationToken));

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
            new GetEquipmentBreakdownReasonsList(page, pageSize, filterColumn, filterValue, sortColumn, isActive),
            cancellationToken));

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetEquipmentBreakdownReasonsByIdQuery(id), cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

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
