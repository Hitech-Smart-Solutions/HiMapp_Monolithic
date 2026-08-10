using Himapp.Execution.Application.Features.ExecutionProjectConfigs.Commands;
using Himapp.Execution.Application.Features.ExecutionProjectConfigs.Models;
using Himapp.Execution.Application.Features.ExecutionProjectConfigs.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Himapp.Execution.Application.Controllers;

[ApiController]
[Route("v1/execution/project-configs")]
public sealed class ExecutionProjectConfigsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ExecutionProjectConfigsController(IMediator mediator) => _mediator = mediator;

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateExecutionProjectConfigRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CreateExecutionProjectConfigCommand(request), cancellationToken);
        return CreatedAtAction(nameof(GetExecutionConfigByProjectID), new { projectId = result.ProjectId }, result);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] UpdateExecutionProjectConfigRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new UpdateExecutionProjectConfigCommand(id, request), cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpGet("GetExecutionConfigByProjectID/{projectId:int}")]
    public async Task<IActionResult> GetExecutionConfigByProjectID(int projectId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetExecutionProjectConfigByProjectIdQuery(projectId),
            cancellationToken);

        return result is null ? NotFound() : Ok(result);
    }
}
