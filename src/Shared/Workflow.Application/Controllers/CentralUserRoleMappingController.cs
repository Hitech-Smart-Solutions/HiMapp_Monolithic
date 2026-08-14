using Himapp.Workflow.Application.Features.CentralUserRoleMapping.Commands;
using Himapp.Workflow.Application.Features.CentralUserRoleMapping.Models;
using Himapp.Workflow.Application.Features.CentralUserRoleMapping.Queries;
using Himapp.Workflow.Application.Features;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Himapp.Workflow.Application.Controllers;

[ApiController]
[Route("v1/workflow/central-user-role-mappings")]
public sealed class CentralUserRoleMappingController : ControllerBase
{
    private readonly IMediator _mediator;

    public CentralUserRoleMappingController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken) =>
        Ok(await _mediator.Send(new GetAllCentralUserRoleMappingsQuery(), cancellationToken));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken) =>
        OkOrNotFound(await _mediator.Send(new GetCentralUserRoleMappingByIdQuery(id), cancellationToken));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCentralUserRoleMappingRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CreateCentralUserRoleMappingCommand(request), cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateCentralUserRoleMappingRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new UpdateCentralUserRoleMappingCommand(id, request), cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, [FromBody] AddTransactionActionHistoryDTO actionHistory, CancellationToken cancellationToken)
    {
        var deleted = await _mediator.Send(new DeleteCentralUserRoleMappingCommand(id, actionHistory), cancellationToken);
        return deleted ? Ok() : NotFound();
    }

    [HttpGet("GetRoleListByProject")]
    public async Task<IActionResult> GetRoleListByProject([FromQuery] SearchParamsProjectWise searchParams, CancellationToken cancellationToken)
    {
        if (searchParams.ProjectID <= 0)
        {
            return BadRequest("ProjectID is required.");
        }

        try
        {
            var result = await _mediator.Send(new GetRoleMappingListByProjectQuery(searchParams), cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }

    }

    private IActionResult OkOrNotFound(object? value) => value is null ? NotFound() : Ok(value);
}
