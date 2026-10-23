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

    /// <summary>Retrieves all specification master records.</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken) =>
        Ok(await _mediator.Send(new GetAllSpecificationMastersQuery(), cancellationToken));

    /// <summary>Retrieves a paginated specification master list with optional filtering, sorting, and active-status selection.</summary>
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

    /// <summary>Retrieves the specification master record with the specified identifier.</summary>
    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetSpecificationMasterByIdQuery(id), cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    /// <summary>Creates a specification master record from the supplied request.</summary>
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

    /// <summary>Updates the specification master record with the specified identifier using the supplied request.</summary>
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

    /// <summary>Changes the active status of the specification master record identified by the supplied action history.</summary>
    [HttpPut("SetActiveInActiveSpecificationMasters")]
    public async Task<IActionResult> Delete([FromBody] AddTransactionActionHistoryDTO dtoInactive, CancellationToken cancellationToken)
    {
        if (dtoInactive is null)
            return BadRequest("Delete request is required.");

        if (dtoInactive.ProgramRowId <= 0)
            return BadRequest("Specification master ID is required.");

        var deleted = await _mediator.Send(new DeleteSpecificationMasterCommand(dtoInactive), cancellationToken);
        return deleted ? Ok() : NotFound();
    }
}
