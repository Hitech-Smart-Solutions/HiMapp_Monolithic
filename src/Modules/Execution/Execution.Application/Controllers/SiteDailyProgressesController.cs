using Himapp.Execution.Application.Features;
using Himapp.Execution.Application.Features.DailyLabor.Commands;
using Himapp.Execution.Application.Features.Manpower.Queries;
using Himapp.Execution.Application.Features.SiteDailyProgress.Commands;
using Himapp.Execution.Application.Features.SiteDailyProgress.Models;
using Himapp.Execution.Application.Features.SiteDailyProgress.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Himapp.Execution.Application.Controllers;

[ApiController]
//[Authorize]
[Route("v1/execution/site-daily-progresses")]
public sealed class SiteDailyProgressesController : ControllerBase
{
    private readonly IMediator _mediator;
    public SiteDailyProgressesController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken) =>
        Ok(await _mediator.Send(new GetAllSiteDailyProgressesQuery(), cancellationToken));

    [AllowAnonymous]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken) =>
        OkOrNotFound(await _mediator.Send(new GetSiteDailyProgressByIdQuery(id), cancellationToken));

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSiteDailyProgressRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CreateSiteDailyProgressCommand(request), cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateSiteDailyProgressRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new UpdateSiteDailyProgressCommand(id, request), cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await _mediator.Send(new DeleteSiteDailyProgressCommand(id), cancellationToken);
        return deleted ? Ok() : NotFound();
    }

    [HttpPut("SetActiveInActiveForSiteDPR")]
    public async Task<IActionResult> SetActiveInActiveForSiteDPR(AddTransactionActionHistoryDTO dto, CancellationToken cancellationToken)
    {
        var deleted = await _mediator.Send(new DeleteSiteDPRCommand(dto), cancellationToken);
        return deleted ? Ok() : NotFound();
    }

    [HttpGet("GetSiteDailyProgressByProjectID")]
    public async Task<IActionResult> GetSiteDailyProgressByProjectID([FromQuery] SearchParamsProjectWise searchParams, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetSiteDailyProgressByProjectIDQuery(searchParams), cancellationToken);
        return Ok(result);
    }


    [HttpGet("GetActivityWiseQuantityBySectionID")]
    public async Task<IActionResult> GetActivityWiseQuantityBySectionID([FromQuery] int areaID, [FromQuery] int projectID, [FromQuery] DateOnly reportDate, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetActivityWiseQuantityBySectionIDQuery
            {
                AreaID = areaID,
                ProjectID = projectID,
                ReportDate = reportDate
            },
            cancellationToken);

        return Ok(result);
    }

    [HttpGet("GetLastSiteDailyProgressBySectionID/{projectId:int}/{sectionId:int}")]
    public async Task<IActionResult> GetLastSiteDailyProgressBySectionID(int projectId, int sectionId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetLastSiteDPRBySectionIDQuery(projectId, sectionId), cancellationToken);

        return result is null ? NotFound() : Ok(result);
    }

    private IActionResult OkOrNotFound(object? value) => value is null ? NotFound() : Ok(value);
}
