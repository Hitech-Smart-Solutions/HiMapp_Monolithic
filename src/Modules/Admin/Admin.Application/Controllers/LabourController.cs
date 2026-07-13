using Himapp.Admin.Application.Features.Labours;
using Himapp.Admin.Application.Features.Labours.Commands;
using Himapp.Admin.Application.Features.Labours.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Himapp.Admin.Application.Controllers;

[ApiController]
[Authorize]
[Route("v1/admin/labours")]
public sealed class LabourController : ControllerBase
{
    private readonly IMediator _mediator;

    public LabourController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<LabourDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetLaboursQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(LabourDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(long id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetLabourByIdQuery(id), cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(LabourDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromForm] LabourFormRequest request, CancellationToken cancellationToken)
    {
        if (request.Photo is null || request.Photo.Length == 0)
        {
            return BadRequest("A labour photo is required.");
        }

        var result = await _mediator.Send(
            new CreateLabourCommand(
                request.ProjectId,
                request.ContractorId,
                request.Name,
                request.DateOfBirth,
                request.AadhaarNumber,
                request.Pan,
                ToUploadedFileInfo(request.Photo)),
            cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:long}")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(LabourDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(long id, [FromForm] LabourFormRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new UpdateLabourCommand(
                id,
                request.ProjectId,
                request.ContractorId,
                request.Name,
                request.DateOfBirth,
                request.AadhaarNumber,
                request.Pan,
                request.Photo is null || request.Photo.Length == 0 ? null : ToUploadedFileInfo(request.Photo)),
            cancellationToken);

        return result is null ? NotFound() : Ok(result);
    }

    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
    {
        var deleted = await _mediator.Send(new DeleteLabourCommand(id), cancellationToken);
        return deleted ? Ok() : NotFound();
    }

    private static UploadedFileInfo ToUploadedFileInfo(IFormFile file) =>
        new(file.FileName, file.ContentType, file.Length);
}

public sealed class LabourFormRequest
{
    public long ProjectId { get; init; }
    public long ContractorId { get; init; }
    public string Name { get; init; } = string.Empty;
    public DateOnly DateOfBirth { get; init; }
    public string AadhaarNumber { get; init; } = string.Empty;
    public string? Pan { get; init; }
    public IFormFile? Photo { get; init; }
}
