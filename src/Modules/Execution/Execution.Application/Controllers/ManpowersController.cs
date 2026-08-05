using Himapp.Execution.Application.Features;
using Himapp.Execution.Application.Features.Activities.Queries;
using Himapp.Execution.Application.Features.Manpower.Commands;
using Himapp.Execution.Application.Features.Manpower.Models;
using Himapp.Execution.Application.Features.Manpower.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Himapp.Execution.Application.Controllers;

[ApiController]
//[Authorize]
[Route("v1/execution/manpowers")]
public sealed class ManpowersController : ControllerBase
{
    private readonly IMediator _mediator;
    public ManpowersController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken) =>
        Ok(await _mediator.Send(new GetAllManpowersQuery(), cancellationToken));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken) =>
        OkOrNotFound(await _mediator.Send(new GetManpowerByIdQuery(id), cancellationToken));

    [HttpGet("GetManpowerByProjectID")]
    public async Task<IActionResult> GetByProjectID([FromQuery] SearchParamsProjectWise searchParams, CancellationToken cancellationToken) =>
       Ok(await _mediator.Send(new GetManpowerByProjectID(searchParams), cancellationToken));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateManpowerRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CreateManpowerCommand(request), cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateManpowerRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new UpdateManpowerCommand(id, request), cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await _mediator.Send(new DeleteManpowerCommand(id), cancellationToken);
        return deleted ? Ok() : NotFound();
    }

    private IActionResult OkOrNotFound(object? value) => value is null ? NotFound() : Ok(value);
}
