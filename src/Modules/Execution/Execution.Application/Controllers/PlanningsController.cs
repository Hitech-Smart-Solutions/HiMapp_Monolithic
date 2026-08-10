using Himapp.Execution.Application.Features;
using Himapp.Execution.Application.Features.Planning.Commands;
using Himapp.Execution.Application.Features.Planning.Models;
using Himapp.Execution.Application.Features.Planning.Queries;
using Himapp.SharedKernel.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Himapp.Execution.Application.Controllers;

[ApiController]
//[Authorize]
[Route("v1/execution/plannings")]
public sealed class PlanningsController : ControllerBase
{
    private readonly IMediator _mediator;
    public PlanningsController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken) =>
        Ok(await _mediator.Send(new GetAllPlanningsQuery(), cancellationToken));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken) =>
        OkOrNotFound(await _mediator.Send(new GetPlanningByIdQuery(id), cancellationToken));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePlanningRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CreatePlanningCommand(request), cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdatePlanningRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new UpdatePlanningCommand(id, request), cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpDelete("{id:int},{deletedBy:int}")]
    public async Task<IActionResult> Delete(int id, int deletedBy, CancellationToken cancellationToken)
    {
        var deleted = await _mediator.Send(new DeletePlanningCommand(id, deletedBy), cancellationToken);
        return deleted ? Ok() : NotFound();
    }


    [HttpGet("GetPlanningListByProject")]
    public async Task<IActionResult> GetPlanningListByProject([FromQuery] SearchParamsProjectWise searchParams, CancellationToken cancellationToken)
    {
        if (searchParams.ProjectID <= 0)
        {
            return BadRequest("ProjectID is required.");
        }

        try
        {
            var result = await _mediator.Send(new GetPlanningListByProjectQuery(searchParams), cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }

    }

    private IActionResult OkOrNotFound(object? value) => value is null ? NotFound() : Ok(value);
}
