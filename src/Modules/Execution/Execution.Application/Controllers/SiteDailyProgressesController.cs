using Himapp.Execution.Application.Features.SiteDailyProgress.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Himapp.Execution.Application.Features.SiteDailyProgress.Queries;
using Himapp.Execution.Application.Features.SiteDailyProgress.Commands;

namespace Himapp.Execution.Application.Controllers;

[ApiController]
[Authorize]
[Route("v1/execution/site-daily-progresses")]
public sealed class SiteDailyProgressesController : ControllerBase
{
    private readonly IMediator _mediator;
    public SiteDailyProgressesController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken) =>
        Ok(await _mediator.Send(new GetAllSiteDailyProgressesQuery(), cancellationToken));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken) =>
        OkOrNotFound(await _mediator.Send(new GetSiteDailyProgressByIdQuery(id), cancellationToken));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSiteDailyProgressRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CreateSiteDailyProgressCommand(request), cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateSiteDailyProgressRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new UpdateSiteDailyProgressCommand(id, request), cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await _mediator.Send(new DeleteSiteDailyProgressCommand(id), cancellationToken);
        return deleted ? Ok() : NotFound();
    }

    private IActionResult OkOrNotFound(object? value) => value is null ? NotFound() : Ok(value);
}
