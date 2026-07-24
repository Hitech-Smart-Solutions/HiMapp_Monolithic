using Himapp.Execution.Application.Features.DailyLabor.Models;
using Himapp.Workflow.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MediatR;

namespace Himapp.Execution.Application.Controllers;

[ApiController]
[Authorize]
[RequiresApproval]
[Route("v1/execution/daily-labors")]
public sealed class DailyLaborsController : ControllerBase
{
    private readonly IMediator _mediator;
    public DailyLaborsController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken) =>
        Ok(await _mediator.Send(new Himapp.Execution.Application.Features.DailyLabor.Queries.GetAllDailyLaborsQuery(), cancellationToken));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken) =>
        OkOrNotFound(await _mediator.Send(new Himapp.Execution.Application.Features.DailyLabor.Queries.GetDailyLaborByIdQuery(id), cancellationToken));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateDailyLaborRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new Himapp.Execution.Application.Features.DailyLabor.Commands.CreateDailyLaborCommand(request), cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateDailyLaborRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new Himapp.Execution.Application.Features.DailyLabor.Commands.UpdateDailyLaborCommand(id, request), cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await _mediator.Send(new Himapp.Execution.Application.Features.DailyLabor.Commands.DeleteDailyLaborCommand(id), cancellationToken);
        return deleted ? Ok() : NotFound();
    }

    private IActionResult OkOrNotFound(object? value) => value is null ? NotFound() : Ok(value);
}
