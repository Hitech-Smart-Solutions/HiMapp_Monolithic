using Himapp.Execution.Application.Features.DailyProgress.Models;
using Himapp.Workflow.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MediatR;
using Himapp.Execution.Application.Features.DailyProgress.Commands;
using Himapp.Execution.Application.Features.DailyProgress.Queries;

namespace Himapp.Execution.Application.Controllers;

[ApiController]
[Authorize]
[RequiresApproval]
[Route("v1/execution/daily-progress")]
public sealed class DailyProgressController : ControllerBase
{
    private readonly IMediator _mediator;
    public DailyProgressController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken) =>
        Ok(await _mediator.Send(new GetAllDailyProgressQuery(), cancellationToken));

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id, CancellationToken cancellationToken) =>
        OkOrNotFound(await _mediator.Send(new GetDailyProgressByIdQuery(id), cancellationToken));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateDailyProgressRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CreateDailyProgressCommand(request), cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, [FromBody] UpdateDailyProgressRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new UpdateDailyProgressCommand(id, request), cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
    {
        var deleted = await _mediator.Send(new DeleteDailyProgressCommand(id), cancellationToken);
        return deleted ? Ok() : NotFound();
    }

    private IActionResult OkOrNotFound(object? value) => value is null ? NotFound() : Ok(value);
}
