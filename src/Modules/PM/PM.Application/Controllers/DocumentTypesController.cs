using Himapp.PM.Application.Features.DocumentTypes.Commands;
using Himapp.PM.Application.Features.DocumentTypes.Models;
using Himapp.PM.Application.Features.DocumentTypes.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Himapp.PM.Application.Controllers;

[ApiController]
[Authorize]
[Route("v1/pm/document-types")]
public sealed class DocumentTypesController : ControllerBase
{
    private readonly IMediator _mediator;
    public DocumentTypesController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken) =>
        Ok(await _mediator.Send(new GetAllDocumentTypesQuery(), cancellationToken));

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetDocumentTypeByIdQuery(id), cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] DocumentTypeRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.DocumentTypeName))
        {
            return BadRequest("Document type name is required.");
        }
        var result = await _mediator.Send(new CreateDocumentTypeCommand(request.DocumentTypeName, request.CreateBy, request.LastModifiedBy, request.Remarks), cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, [FromBody] DocumentTypeRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new UpdateDocumentTypeCommand(id, request.DocumentTypeName, request.LastModifiedBy, request.Remarks), cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
    {
        var deleted = await _mediator.Send(new DeleteDocumentTypeCommand(id), cancellationToken);
        return deleted ? Ok() : NotFound();
    }
}
