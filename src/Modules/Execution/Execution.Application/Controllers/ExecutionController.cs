using Himapp.Execution.Application.Features.Activities;
using Himapp.Execution.Application.Features.Activities.Commands;
using Himapp.Execution.Application.Features.Activities.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Himapp.Execution.Application.Controllers;

[ApiController]
[Authorize]
[Route("v1/execution/activities")]
public sealed class ExecutionController : ControllerBase
{
    private readonly IMediator _mediator;

    public ExecutionController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken) =>
        Ok(await _mediator.Send(new GetAllActivitiesQuery(), cancellationToken));

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetActivityByIdQuery(id), cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ActivityRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.ActivityCode))
        {
            return BadRequest("Activity code is required.");
        }

        var result = await _mediator.Send(
            new CreateActivityCommand(request.ProjectId, request.ActivityCode, request.Description, request.ProgressPercent, request.WorkDate),
            cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, [FromBody] ActivityRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new UpdateActivityCommand(id, request.ProjectId, request.ActivityCode, request.Description, request.ProgressPercent, request.WorkDate),
            cancellationToken);

        return result is null ? NotFound() : Ok(result);
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
    {
        var deleted = await _mediator.Send(new DeleteActivityCommand(id), cancellationToken);
        return deleted ? Ok() : NotFound();
    }
}
