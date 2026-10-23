using Himapp.PM.Application.Features.WorkDescriptions.Commands;
using Himapp.PM.Application.Features.WorkDescriptions.Models;
using Himapp.PM.Application.Features.WorkDescriptions.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Himapp.PM.Application.Controllers;

[ApiController]
[Authorize]
[Route("v1/pm/work-descriptions")]
public sealed class WorkDescriptionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public WorkDescriptionsController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken) =>
        Ok(await _mediator.Send(new GetAllWorkDescriptionsQuery(), cancellationToken));

    [HttpGet("list")]
    public async Task<IActionResult> GetList(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? filterColumn = null,
        [FromQuery] string? filterValue = null,
        [FromQuery] string? sortColumn = "ID ASC",
        [FromQuery] string? isActive = "all",
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(
            new GetWorkDescriptionList(page, pageSize, filterColumn, filterValue, sortColumn, isActive),
            cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetWorkDescriptionByIdQuery(id), cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] WorkDescriptionRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.WorkDescriptionName))
            return BadRequest("Work description name is required.");

        var result = await _mediator.Send(
            new CreateWorkDescriptionCommand(request.WorkDescriptionName, request.IsActive, request.CreateBy, request.LastModifiedBy),
            cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, [FromBody] WorkDescriptionRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.WorkDescriptionName))
            return BadRequest("Work description name is required.");

        var result = await _mediator.Send(
            new UpdateWorkDescriptionCommand(id, request.WorkDescriptionName, request.IsActive, request.LastModifiedBy),
            cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
    {
        var deleted = await _mediator.Send(new DeleteWorkDescriptionCommand(id), cancellationToken);
        return deleted ? Ok() : NotFound();
    }
}
