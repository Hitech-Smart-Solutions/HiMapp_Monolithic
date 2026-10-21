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

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken) =>
        Ok(await _mediator.Send(new GetAllEquipmentChecklistPointsQuery(), cancellationToken));

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetEquipmentChecklistPointByIdQuery(id), cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

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

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await _mediator.Send(new DeleteEquipmentChecklistPointCommand(id), cancellationToken);
        return deleted ? Ok() : NotFound();
    }
}
