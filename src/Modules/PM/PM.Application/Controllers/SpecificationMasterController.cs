using Himapp.PM.Application.Features;
using Himapp.PM.Application.Features.SpecificationMaster.Commands;
using Himapp.PM.Application.Features.SpecificationMaster.Models;
using Himapp.PM.Application.Features.SpecificationMaster.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Himapp.PM.Application.Controllers;

[ApiController]
[Authorize]
[Route("v1/pm/specification-master")]
public sealed class SpecificationMasterController : ControllerBase
{
    private readonly IMediator _mediator;

    public SpecificationMasterController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken) =>
        Ok(await _mediator.Send(new GetAllSpecificationMastersQuery(), cancellationToken));

    [HttpGet("list")]
    public async Task<IActionResult> GetList(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? filterColumn = null,
        [FromQuery] string? filterValue = null,
        [FromQuery] string? sortColumn = "ID ASC",
        [FromQuery] string? isActive = "all",
        CancellationToken cancellationToken = default) =>
        Ok(await _mediator.Send(
            new GetSpecificationMasterList(page, pageSize, filterColumn, filterValue, sortColumn, isActive),
            cancellationToken));

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetSpecificationMasterByIdQuery(id), cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] SpecificationMasterRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.SpecificationName))
            return BadRequest("Specification name is required.");

        if (!Enum.IsDefined(request.Type))
            return BadRequest("Specification type must be Numeric, AlphaNumeric, or DateAndTime.");

        var result = await _mediator.Send(
            new CreateSpecificationMasterCommand(
                request.SpecificationName,
                request.IsActive,
                request.Type,
                request.CreateBy,
                request.LastModifiedBy),
            cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(int id, [FromBody] SpecificationMasterRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.SpecificationName))
            return BadRequest("Specification name is required.");

        if (!Enum.IsDefined(request.Type))
            return BadRequest("Specification type must be Numeric, AlphaNumeric, or DateAndTime.");

        var result = await _mediator.Send(
            new UpdateSpecificationMasterCommand(
                id,
                request.SpecificationName,
                request.IsActive,
                request.Type,
                request.LastModifiedBy),
            cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await _mediator.Send(new DeleteSpecificationMasterCommand(id), cancellationToken);
        return deleted ? Ok() : NotFound();
    }
}
