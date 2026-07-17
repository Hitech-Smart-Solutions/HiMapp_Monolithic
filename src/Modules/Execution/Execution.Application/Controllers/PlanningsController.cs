using Himapp.Execution.Application.Features.Planning.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MediatR;

namespace Himapp.Execution.Application.Controllers;

[ApiController]
[Authorize]
[Route("v1/execution/plannings")]
public sealed class PlanningsController : ControllerBase
{
    private readonly IMediator _mediator;
    public PlanningsController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken) =>
        Ok(await _mediator.Send(new Himapp.Execution.Application.Features.Planning.Queries.GetAllPlanningsQuery(), cancellationToken));

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id, CancellationToken cancellationToken) =>
        OkOrNotFound(await _mediator.Send(new Himapp.Execution.Application.Features.Planning.Queries.GetPlanningByIdQuery(id), cancellationToken));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePlanningRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new Himapp.Execution.Application.Features.Planning.Commands.CreatePlanningCommand(request), cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, [FromBody] UpdatePlanningRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new Himapp.Execution.Application.Features.Planning.Commands.UpdatePlanningCommand(id, request), cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
    {
        var deleted = await _mediator.Send(new Himapp.Execution.Application.Features.Planning.Commands.DeletePlanningCommand(id), cancellationToken);
        return deleted ? Ok() : NotFound();
    }

    private IActionResult OkOrNotFound(object? value) => value is null ? NotFound() : Ok(value);
}
