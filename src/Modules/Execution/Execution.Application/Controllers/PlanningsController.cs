using Himapp.Execution.Application.Features;
using Himapp.Execution.Application.Features.Planning.Commands;
using Himapp.Execution.Application.Features.Planning.Models;
using Himapp.Execution.Application.Features.Planning.Queries;
using Himapp.SharedKernel.Abstractions;
using Himapp.Execution.Application.Features.Planning.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Himapp.Execution.Application.Controllers;

[ApiController]
[Authorize]
[Route("v1/execution/plannings")]
public sealed class PlanningsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICurrentUser _currentUser;
    public PlanningsController(IMediator mediator, ICurrentUser currentUser) => (_mediator, _currentUser) = (mediator, currentUser);

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

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is not int userId) return Unauthorized();
        var deleted = await _mediator.Send(new DeletePlanningCommand(id, userId), cancellationToken);
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
        catch (Exception)
        {
            return StatusCode(500, "An unexpected error occurred.");
        }

    }


    [HttpGet("download-template")]
    public async Task<IActionResult> DownloadTemplate([FromQuery] int projectId, CancellationToken cancellationToken)
    {
        if (projectId <= 0) return BadRequest("ProjectId is required.");

        try
        {
            var bytes = await _mediator.Send(new DownloadPlanningTemplateQuery(projectId), cancellationToken);
            var fileName = $"Planning_Bulk_Upload_Template_{projectId}.xlsx";
            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception)
        {
            return StatusCode(500, "An unexpected error occurred.");
        }
    }


    [HttpPost("bulk")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> BulkCreate([FromForm] BulkCreatePlanningRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _mediator.Send(new BulkCreatePlanningCommand(request), cancellationToken);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            // validation errors from handler (parse errors)
            var parts = ex.Message.Split(new[] { "||" }, StringSplitOptions.RemoveEmptyEntries);
            return BadRequest(parts);
        }
        catch (Exception)
        {
            return StatusCode(500, "An unexpected error occurred.");
        }
    }

    private IActionResult OkOrNotFound(object? value) => value is null ? NotFound() : Ok(value);
}
