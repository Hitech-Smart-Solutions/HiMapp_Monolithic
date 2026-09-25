using Himapp.Execution.Application.Features;
using Himapp.Execution.Application.Features.LockRequest.Commands;
using Himapp.Execution.Application.Features.LockRequest.Models;
using Himapp.Execution.Application.Features.LockRequest.Queries;
using Himapp.Workflow.Application.Filters;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Himapp.Execution.Application.Controllers;

[ApiController]
[Authorize]
[Route("v1/execution/lock-requests")]
public sealed class LockRequestsController : ControllerBase
{
    private readonly IMediator _mediator;
    public LockRequestsController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken) =>
        Ok(await _mediator.Send(new GetAllLockRequestsQuery(), cancellationToken));

    [HttpGet("GetLockOpenRequestById/{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken) =>
        OkOrNotFound(await _mediator.Send(new GetLockRequestByIdQuery(id), cancellationToken));

    [HttpGet("GetLockRequestByProjectID")]
    public async Task<IActionResult> GetLockRequestByProjectID([FromQuery] SearchParamsProjectWise searchParams, CancellationToken cancellationToken) =>
       Ok(await _mediator.Send(new GetLockRequestByProjectIdQuery(searchParams), cancellationToken));

    [HttpPost("CreateLockOpenRequest")]
    [RequiresApproval(programId: 98, priority: 0)]
    public async Task<IActionResult> Create([FromBody] CreateLockRequestRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CreateLockRequestCommand(request), cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("UpdateLockOpenRequestById/{id:int}")]
    [RequiresApproval(programId: 98, priority: 0)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateLockRequestRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new UpdateLockRequestCommand(id, request), cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await _mediator.Send(new DeleteLockRequestCommand(id), cancellationToken);
        return deleted ? Ok() : NotFound();
    }

    [HttpPut("SetActiveInActiveForLockRequest")]
    public async Task<IActionResult> SetActiveInActiveForLockRequest(AddTransactionActionHistoryDTO dto, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DeleteLockRequestActionCommand(dto), cancellationToken);
        return result ? Ok() : NotFound();
    }

    [HttpGet("GetLockOpenRequestByIdAndProgramId")]
    public async Task<IActionResult> GetLockOpenRequestByIdAndProgramId(int id,int programId,CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetLockOpenRequestByIdAndProgramIdQuery(id, programId),
            cancellationToken);

        if (result is null)
        {
            return NotFound();
        }

        return Ok(result);
    }
    private IActionResult OkOrNotFound(object? value) => value is null ? NotFound() : Ok(value);
}
