using Himapp.PM.Application.Features.SpareParts.Commands;
using Himapp.PM.Application.Features.SpareParts.Queries;
using Himapp.PM.Application.Features.SpareParts.Models;
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
                    request.EquipmentcategoryID,
                    request.BrandName,
                    request.ModelName,
                    request.DocumentName,
                    request.DocumentPath,
                    request.StatusID,
                    request.LastModifiedBy),
                cancellationToken);

            return result is null ? NotFound() : Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await _mediator.Send(new DeleteSparePartCatalogueCommand(id), cancellationToken);
        return deleted ? Ok() : NotFound();
    }
}
