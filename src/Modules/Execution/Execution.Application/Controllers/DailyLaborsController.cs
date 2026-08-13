using Himapp.Execution.Application.Features;
using Himapp.Execution.Application.Features.DailyLabor.Commands;
using Himapp.Execution.Application.Features.DailyLabor.Models;
using Himapp.Execution.Application.Features.DailyLabor.Queries;
using Himapp.Execution.Application.Features.Manpower.Queries;
using Himapp.Workflow.Filters;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Himapp.Execution.Application.Controllers;

[ApiController]
//[Authorize]
[Route("v1/execution/daily-labors")]
public sealed class DailyLaborsController : ControllerBase
{
    private readonly IMediator _mediator;
    public DailyLaborsController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken) =>
        Ok(await _mediator.Send(new GetAllDailyLaborsQuery(), cancellationToken));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken) =>
        OkOrNotFound(await _mediator.Send(new GetDailyLaborByIdQuery(id), cancellationToken));


    [HttpGet("GetConsolidated")]
    public async Task<IActionResult> GetConsolidated([FromQuery] int projectId, [FromQuery] DateOnly date, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetConsolidatedDailyLaborQuery(projectId,date),cancellationToken);

        return Ok(result);
    }

    [HttpGet("GetDailyLaborByProjectID")]
    public async Task<IActionResult> GetByProjectID([FromQuery] SearchParamsProjectWise searchParams, CancellationToken cancellationToken) =>
       Ok(await _mediator.Send(new GetDailyLaborByProjectID(searchParams), cancellationToken));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateDailyLaborRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CreateDailyLaborCommand(request), cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateDailyLaborRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new UpdateDailyLaborCommand(id, request), cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await _mediator.Send(new DeleteDailyLaborCommand(id), cancellationToken);
        return deleted ? Ok() : NotFound();
    }

    [HttpPut("SetActiveInActiveForDailyLabour")]
    public async Task<IActionResult> SetActiveInActiveForDailyLabour(AddTransactionActionHistoryDTO dto, CancellationToken cancellationToken)
    {
        var deleted = await _mediator.Send(new DeleteDailyLaborActionCommand(dto), cancellationToken);
        return deleted ? Ok() : NotFound();
    }

    private IActionResult OkOrNotFound(object? value) => value is null ? NotFound() : Ok(value);
}
