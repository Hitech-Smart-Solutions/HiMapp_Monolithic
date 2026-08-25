using Himapp.Execution.Application.Features.DailyDepartmentalLabourSlip.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Himapp.Execution.Application.Features;
using Himapp.Execution.Application.Features.DailyDepartmentalLabourSlip.Queries;
using Himapp.Execution.Application.Features.DailyDepartmentalLabourSlip.Commands;
using Himapp.Workflow.Application.Filters;

namespace Himapp.Execution.Application.Controllers;

[ApiController]
[Authorize]

[Route("v1/execution/daily-departmental-labour-slips")]
public sealed class DailyDepartmentalLabourSlipsController : ControllerBase
{
    private readonly IMediator _mediator;
    public DailyDepartmentalLabourSlipsController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken) =>
        Ok(await _mediator.Send(new GetAllDailyDepartmentalLabourSlipsQuery(), cancellationToken));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken) =>
        OkOrNotFound(await _mediator.Send(new GetDailyDepartmentalLabourSlipByIdQuery(id), cancellationToken));

    [RequiresApprovalAttribute]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateDailyDepartmentalLabourSlipRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CreateDailyDepartmentalLabourSlipCommand(request), cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateDailyDepartmentalLabourSlipRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new UpdateDailyDepartmentalLabourSlipCommand(id, request), cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await _mediator.Send(new DeleteDailyDepartmentalLabourSlipCommand(id), cancellationToken);
        return deleted ? Ok() : NotFound();
    }

    [HttpPut("SetActiveInActiveForDDLS")]
    public async Task<IActionResult> SetActiveInActiveForDDLS(AddTransactionActionHistoryDTO dto, CancellationToken cancellationToken)
    {
        var deleted = await _mediator.Send(new DeleteDDLSCommand(dto), cancellationToken);
        return deleted ? Ok() : NotFound();
    }

    [HttpGet("GetDailyDepartmentalLabourSlipsByProjectID")]
    public async Task<IActionResult> GetDailyDepartmentalLabourSlipsByProjectID([FromQuery] SearchParamsProjectWise searchParams, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetDailyDepartmentalLabourSlipsByProjectID(searchParams), cancellationToken);
        return Ok(result);
    }

    private IActionResult OkOrNotFound(object? value) => value is null ? NotFound() : Ok(value);
}
