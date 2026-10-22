using Himapp.PM.Application.Features.EquipmentDocuments.Commands;
using Himapp.PM.Application.Features.EquipmentDocuments.Models;
using Himapp.PM.Application.Features.EquipmentDocuments.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Himapp.PM.Application.Controllers;

[ApiController]
[Authorize]
[Route("v1/pm/equipment-documents")]
public sealed class EquipmentDocumentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public EquipmentDocumentsController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken) =>
        Ok(await _mediator.Send(new GetAllEquipmentDocumentsQuery(), cancellationToken));

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
            new GetEquipmentDocumentList(page, pageSize, filterColumn, filterValue, sortColumn, isActive),
            cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetEquipmentDocumentByIdQuery(id), cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] EquipmentDocumentRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new CreateEquipmentDocumentCommand(
                request.DocumentTypeID,
                request.DocumentName,
                request.StartDate,
                request.EndDate,
                request.IsActive,
                request.CreateBy,
                request.LastModifiedBy),
            cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] EquipmentDocumentRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new UpdateEquipmentDocumentCommand(
                id,
                request.DocumentTypeID,
                request.DocumentName,
                request.StartDate,
                request.EndDate,
                request.IsActive,
                request.LastModifiedBy),
            cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteEquipmentDocumentCommand(id), cancellationToken);
        return Ok();
    }
}
