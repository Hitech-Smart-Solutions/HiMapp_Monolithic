using Himapp.Execution.Application.Features.Activities;
using Himapp.Execution.Application.Features.Activities.Commands;
using Himapp.Execution.Application.Features.Activities.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Himapp.Execution.Application.Controllers;

[ApiController]
//[Authorize]
[Route("v1/execution/activities")]
public sealed class ActivityController : ControllerBase
{
    private readonly IMediator _mediator;

    public ActivityController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken) =>
        Ok(await _mediator.Send(new GetAllActivitiesQuery(), cancellationToken));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetActivityByIdQuery(id), cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ActivityRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.ActivityName))
        {
            return BadRequest("Activity Name is required.");
        }

        var result = await _mediator.Send(
            new CreateActivityCommand(request.CompanyID, request.ActivityName, request.UOMID, request.CreateBy, request.LastModifiedBy),
            cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] ActivityRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new UpdateActivityCommand(id,request.ActivityName, request.UOMID, request.LastModifiedBy),
            cancellationToken);

        return result is null ? NotFound() : Ok(result);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await _mediator.Send(new DeleteActivityCommand(id), cancellationToken);
        return deleted ? Ok() : NotFound();
    }
}
