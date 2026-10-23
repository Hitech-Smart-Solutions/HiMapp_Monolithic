using Himapp.PM.Application.Features.PreventiveMaintenancePoints.Commands;
using Himapp.PM.Application.Features.SpareParts.Commands;
using Himapp.PM.Application.Features.SpareParts.Models;
using Himapp.PM.Application.Features.SpareParts.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Himapp.PM.Application.Controllers;

[ApiController]
[Authorize]
[Route("v1/pm/spare-part-catalogues")]
public sealed class SparePartCataloguesController : ControllerBase
{
    private readonly IMediator _mediator;

    public SparePartCataloguesController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken) =>
        Ok(await _mediator.Send(new GetAllSparePartCataloguesQuery(), cancellationToken));

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
            new GetSparepartListQuery(page, pageSize, filterColumn, filterValue, sortColumn, isActive),
            cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetSparePartCatalogueByIdQuery(id), cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] SparePartCatalogueRequest request, CancellationToken cancellationToken)
    {
        if (request == null)
        {
            return BadRequest("Request body cannot be null.");
        }

        try
        {
            var result = await _mediator.Send(
                new CreateSparePartCatalogueCommand(
                    request.EquipmentcategoryID,
                    request.BrandName,
                    request.ModelName,
                    request.DocumentName,
                    request.DocumentPath,
                    request.StatusID,
                    request.CreateBy,
                    request.LastModifiedBy),
                cancellationToken);

            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] SparePartCatalogueRequest request, CancellationToken cancellationToken)
    {
        if (request == null)
        {
            return BadRequest("Request body cannot be null.");
        }

        try
        {
            var result = await _mediator.Send(
                new UpdateSparePartCatalogueCommand(
                    id,
                    request.DocumentName,
                    request.DocumentPath,
                    request.LastModifiedBy),
                cancellationToken);

            return result is null ? NotFound() : Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPut("SetActiveInActiveSpartPartCatalogues")]
    public async Task<IActionResult> Delete([FromBody] AddTransactionActionHistoryDTO dtoInactive, CancellationToken cancellationToken)
    {
        if (dtoInactive is null)
            return BadRequest("Delete request is required.");

        if (dtoInactive.ProgramRowId <= 0)
            return BadRequest("Spare part catalogue ID is required.");

        var deleted = await _mediator.Send(new DeleteSparePartCatalogueCommand(dtoInactive), cancellationToken);
        return deleted ? Ok() : NotFound();
    }
}
