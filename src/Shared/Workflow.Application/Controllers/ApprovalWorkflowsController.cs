using Himapp.Workflow.Application.Features.ApprovalWorkflow.Commands;
using Himapp.Workflow.Application.Features.ApprovalWorkflow.Models;
using Himapp.Workflow.Application.Features.ApprovalWorkflow.Queries;
using Himapp.Workflow.Application.Features;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Himapp.Workflow.Application.Controllers;

[ApiController]
[Route("v1/workflow/approval-workflows")]
public sealed class ApprovalWorkflowsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ApprovalWorkflowsController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken) =>
        Ok(await _mediator.Send(new GetAllApprovalWorkflowsQuery(), cancellationToken));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken) =>
        OkOrNotFound(await _mediator.Send(new GetApprovalWorkflowByIdQuery(id), cancellationToken));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateApprovalWorkflowRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CreateApprovalWorkflowCommand(request), cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateApprovalWorkflowRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new UpdateApprovalWorkflowCommand(id, request), cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, [FromBody] AddTransactionActionHistoryDTO actionHistory, CancellationToken cancellationToken)
    {
        var deleted = await _mediator.Send(new DeleteApprovalWorkflowCommand(id, actionHistory), cancellationToken);
        return deleted ? Ok() : NotFound();
    }

    [HttpGet("GetApprovalWorkflowByCompany")]
    public async Task<IActionResult> GetWorkflowListByCompany([FromQuery] SearchParams searchParams, CancellationToken cancellationToken)
    {
        if (searchParams is null)
        {
            return BadRequest("Search parameters are required.");
        }

        try
        {
            var result = await _mediator.Send(new GetWorkflowListByCompanyQuery(searchParams), cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }

    }

    private IActionResult OkOrNotFound(object? value) => value is null ? NotFound() : Ok(value);
}
