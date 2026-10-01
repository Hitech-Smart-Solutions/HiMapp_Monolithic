using Himapp.Execution.Application.Features;
using Himapp.Execution.Application.Features.DailyLabor.Commands;
using Himapp.Execution.Application.Features.DailyLabor.Models;
using Himapp.Execution.Application.Features.DailyLabor.Queries;
using Himapp.Execution.Application.Features.Manpower.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Himapp.Execution.Application.Controllers;

[ApiController]
[Authorize]
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
        var result = await _mediator.Send(new GetConsolidatedDailyLaborQuery(projectId, date), cancellationToken);

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

    [HttpGet("GetConsolidatedForDPR")]
    public async Task<IActionResult> GetConsolidatedForDPR([FromQuery] DateOnly date, [FromQuery] int projectId, [FromQuery] int Id, CancellationToken cancellationToken)
    {
        if (projectId <= 0)
        {
            return BadRequest("ProjectID is required.");
        }

        if (date == default)
        {
            return BadRequest("date is required.");
        }

        var result = await _mediator.Send(new DPRGetConsolidatedDailyLaborQuery(date, projectId,Id), cancellationToken);

        return result is null ? NotFound() : Ok(result);
    }
    [HttpGet("GetDailyDPRReport")]
    public async Task<IActionResult> GetDailyDPRReport(
    [FromQuery] int type = 0,
    [FromQuery] DateTime? fromDate = null,
    [FromQuery] DateTime? toDate = null,
    [FromQuery] int project = 0,
    [FromQuery] int activity = 0,
    [FromQuery] int contractor = 0,
    [FromQuery] int section = 0,
    [FromQuery] bool? departmental = null,
    CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(
            new GetProjectDPRReport(
                Type: type,
                FromDate: fromDate,
                ToDate: toDate,
                Project: project,
                Activity: activity,
                Contractor: contractor,
                Section: section,
                Departmental: departmental
            ),
            cancellationToken);
        return Ok(result);
    }
    
    [HttpGet("GetDailyLaborContractorsByProjectAndDate")]
    public async Task<IActionResult> GetDailyLaborContractorsByProjectAndDate([FromQuery] int projectId, [FromQuery] DateOnly date, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetDailyLaborContractorsByProjectAndDateQuery(projectId, date), cancellationToken);

        return Ok(result);
    }
    private IActionResult OkOrNotFound(object? value) => value is null ? NotFound() : Ok(value);
}
