using Himapp.Safety.Application.Features.Incidents;
using Himapp.Safety.Application.Features.Incidents.Commands;
using Himapp.Safety.Application.Features.Incidents.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Himapp.Safety.Application.Controllers;

[ApiController]
[Authorize]
[Route("v1/safety/incidents")]
public sealed class SafetyController : ControllerBase
{
    private readonly IMediator _mediator;

    public SafetyController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken) =>
        Ok(await _mediator.Send(new GetAllIncidentsQuery(), cancellationToken));

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetIncidentByIdQuery(id), cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Create([FromForm] IncidentFormRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
        {
            return BadRequest("Incident title is required.");
        }

        var result = await _mediator.Send(
            new CreateIncidentCommand(request.ProjectId, request.Title, request.Severity, request.OccurredOn, request.Description, ToUploadedFileInfo(request.Attachment)),
            cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:long}")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Update(long id, [FromForm] IncidentFormRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new UpdateIncidentCommand(id, request.ProjectId, request.Title, request.Severity, request.OccurredOn, request.Description, ToUploadedFileInfo(request.Attachment)),
            cancellationToken);

        return result is null ? NotFound() : Ok(result);
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
    {
        var deleted = await _mediator.Send(new DeleteIncidentCommand(id), cancellationToken);
        return deleted ? Ok() : NotFound();
    }

    private static UploadedFileInfo? ToUploadedFileInfo(IFormFile? file) =>
        file is null || file.Length == 0 ? null : new UploadedFileInfo(file.FileName, file.ContentType, (int)file.Length);
}

public sealed class IncidentFormRequest
{
    public long ProjectId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Severity { get; init; } = "Low";
    public DateOnly OccurredOn { get; init; }
    public string Description { get; init; } = string.Empty;
    public IFormFile? Attachment { get; init; }
}
