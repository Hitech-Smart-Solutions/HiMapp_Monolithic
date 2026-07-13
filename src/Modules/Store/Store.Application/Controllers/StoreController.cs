using Himapp.Store.Application.Features.GatePasses;
using Himapp.Store.Application.Features.GatePasses.Commands;
using Himapp.Store.Application.Features.GatePasses.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Himapp.Store.Application.Controllers;

[ApiController]
[Authorize]
[Route("v1/store/gatepasses")]
public sealed class StoreController : ControllerBase
{
    private readonly IMediator _mediator;

    public StoreController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<GatePassDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAllGatePassesQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(GatePassDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(long id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetGatePassByIdQuery(id), cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(GatePassDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromForm] GatePassFormRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.GatePassNo))
        {
            return BadRequest("Gate pass number is required.");
        }

        var result = await _mediator.Send(
            new CreateGatePassCommand(
                request.ProjectId,
                request.GatePassNo,
                request.Path,
                request.ServiceRequestId,
                request.BackdatedReason,
                request.Lines,
                ToUploadedFileInfo(request.SupportingDocument)),
            cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:long}")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(GatePassDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(long id, [FromForm] GatePassFormRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.GatePassNo))
        {
            return BadRequest("Gate pass number is required.");
        }

        var result = await _mediator.Send(
            new UpdateGatePassCommand(
                id,
                request.ProjectId,
                request.GatePassNo,
                request.Path,
                request.ServiceRequestId,
                request.BackdatedReason,
                request.Lines,
                ToUploadedFileInfo(request.SupportingDocument)),
            cancellationToken);

        return result is null ? NotFound() : Ok(result);
    }

    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
    {
        var deleted = await _mediator.Send(new DeleteGatePassCommand(id), cancellationToken);
        return deleted ? Ok() : NotFound();
    }

    private static UploadedFileInfo? ToUploadedFileInfo(IFormFile? file) =>
        file is null || file.Length == 0
            ? null
            : new UploadedFileInfo(file.FileName, file.ContentType, file.Length);
}

public sealed class GatePassFormRequest
{
    public long ProjectId { get; init; }
    public string GatePassNo { get; init; } = string.Empty;
    public string Path { get; init; } = "A";
    public long? ServiceRequestId { get; init; }
    public string? BackdatedReason { get; init; }
    public List<GatePassLineRequest> Lines { get; init; } = [];
    public IFormFile? SupportingDocument { get; init; }
}
