using DocumentFormat.OpenXml.Office2010.Excel;
using Himapp.Execution.Application.Features;
using Himapp.Execution.Application.Features.DailyProgress.Commands;
using Himapp.Execution.Application.Features.DailyProgress.Models;
using Himapp.Execution.Application.Features.DailyProgress.Queries;
using Himapp.Execution.Application.Features.Planning.Queries;
using Himapp.Execution.Application.Features.SiteDailyProgress.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Himapp.Execution.Application.Controllers;

[ApiController]
//[Authorize]
[Route("v1/execution/daily-progress")]
public sealed class DailyProgressController : ControllerBase
{
    private readonly IMediator _mediator;
    public DailyProgressController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken) =>
        Ok(await _mediator.Send(new GetAllDailyProgressQuery(), cancellationToken));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        if (id <= 0)
        {
            return BadRequest("Project DPR ID is required.");
        }

        return OkOrNotFound(await _mediator.Send(new GetDailyProgressByIdQuery(id), cancellationToken));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateDailyProgressRequest request, CancellationToken cancellationToken)
    {
        if (request == null)
        {
            return BadRequest("Create request is required.");
        }
        else if (request.ReportDate == default)
        {
            return BadRequest("Report date is required.");
        }

        var result = await _mediator.Send(new CreateDailyProgressCommand(request), cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateDailyProgressRequest request, CancellationToken cancellationToken)
    {
        if (request == null)
        {
            return BadRequest("Update request is required.");
        }
        else if (request.Id != id)
        {
            return BadRequest("Project DPR ID does not match.");
        }

        var result = await _mediator.Send(new UpdateDailyProgressCommand(id, request), cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPut("SetActiveInActiveDailyProgress")]
    public async Task<IActionResult> Delete([FromBody] AddTransactionActionHistoryDTO dtoInactive, CancellationToken cancellationToken)
    {
        if (dtoInactive == null)
        {
            return BadRequest("Delete request is required.");
        }
        else if (dtoInactive.ProgramId <= 0)
        {
            return BadRequest("Program ID is required.");
        }
        else if (dtoInactive.ProgramRowId <= 0)
        {
            return BadRequest("Daily Progress ID is required.");
        }

        var deleted = await _mediator.Send(new DeleteDailyProgressCommand(dtoInactive), cancellationToken);
        return deleted ? Ok() : NotFound();
    }

    [HttpGet("GetProjectDailyProgressListByProject")]
    public async Task<IActionResult> GetDailyProgressListByProject([FromQuery] SearchParamsProjectWise searchParams, CancellationToken cancellationToken)
    {
        if (searchParams.ProjectID <= 0)
        {
            return BadRequest("ProjectID is required.");
        }

        try
        {
            var result = await _mediator.Send(new GetDailyProgressListByProjectQuery(searchParams), cancellationToken);
            return Ok(result);
        }
        catch (Exception)
        {
            return StatusCode(500, "An unexpected error occurred.");
        }

    }

    [HttpGet("GetActivityWiseQuantityByProjectID/{projectId:int}")]
    public async Task<IActionResult> GetActivityWiseQuantityByProjectID(int projectId, CancellationToken cancellationToken)
    {
        if (projectId <= 0)
        {
            return BadRequest("ProjectID is required.");
        }

        try
        {
            var result = await _mediator.Send(new GetActivityWiseQuantityByProjectQuery(projectId), cancellationToken);
            return Ok(result);
        }
        catch (Exception)
        {
            return StatusCode(500, "An unexpected error occurred.");
        }
    }

    [HttpGet("GetByProjectAndDate/{projectId:int}/{reportDate}")]
    public async Task<IActionResult> GetByProjectAndDate(int projectId, DateOnly reportDate, CancellationToken cancellationToken)
    {
        if (projectId <= 0)
        {
            return BadRequest("ProjectID is required.");
        }

        if (reportDate == default)
        {
            return BadRequest("Report date is required.");
        }

        var result = await _mediator.Send(new GetDailyProgressByProjectAndDateQuery(projectId, reportDate), cancellationToken);

        return result is null ? NotFound() : Ok(result);
    }

    private IActionResult OkOrNotFound(object? value) => value is null ? NotFound() : Ok(value);
}
