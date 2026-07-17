using Himapp.Execution.Application.Features.RateMaster.Commands;
using Himapp.Execution.Application.Features.RateMaster.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Himapp.Execution.Application.Features.RateMaster.Models;

namespace Himapp.Execution.Application.Controllers;

[ApiController]
[Authorize]
[Route("v1/execution/rate-masters")]
public sealed class RateMastersController : ControllerBase
{
    private readonly IMediator _mediator;
    public RateMastersController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken) => Ok(await _mediator.Send(new GetAllRateMastersQuery(), cancellationToken));

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetRateMasterByIdQuery(id), cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateRateMasterRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CreateRateMasterCommand(request), cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, [FromBody] UpdateRateMasterRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new UpdateRateMasterCommand(id, request), cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
    {
        var deleted = await _mediator.Send(new DeleteRateMasterCommand(id), cancellationToken);
        return deleted ? Ok() : NotFound();
    }
}
