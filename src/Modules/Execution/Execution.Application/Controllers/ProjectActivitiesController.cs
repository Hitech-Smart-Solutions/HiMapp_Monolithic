using Himapp.Execution.Application.Features;
using Himapp.Execution.Application.Features.ProjectActivities.Commands;
using Himapp.Execution.Application.Features.ProjectActivities.Models;
using Himapp.Execution.Application.Features.ProjectActivities.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Himapp.Execution.Application.Controllers;

[ApiController]
[Authorize]
[Route("v1/execution/project-activities")]
public sealed class ProjectActivitiesController : ControllerBase
{
    private readonly IMediator _mediator;
    public ProjectActivitiesController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] SearchParamsCompanyProjectWise searchParams, CancellationToken cancellationToken) => 
        Ok(await _mediator.Send(new GetAllProjectActivitiesQuery(searchParams), cancellationToken));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetProjectActivityByIdQuery(id), cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProjectActivityRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CreateProjectActivityCommand(request), cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateProjectActivityRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new UpdateProjectActivityCommand(id, request), cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpDelete("{id:int}/{projectid:int}")]
    public async Task<IActionResult> Delete(int id,int projectid, CancellationToken cancellationToken)
    {
        var deleted = await _mediator.Send(new DeleteProjectActivityCommand(id, projectid), cancellationToken);
        return deleted ? Ok() : NotFound();
    }

    [HttpGet("GetProjectActivityByProjectId/{projectid:int}")]
    public async Task<IActionResult> GetProjectActivityByProjectId(int projectid, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetProjectActivityByProjectIdQuery(projectid), cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }
}
