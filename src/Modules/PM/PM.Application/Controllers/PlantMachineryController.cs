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

    public PlantMachineryController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken) =>
        Ok(await _mediator.Send(new GetAllEquipmentsQuery(), cancellationToken));

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
}
