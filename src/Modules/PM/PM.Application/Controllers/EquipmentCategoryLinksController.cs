using Himapp.PM.Application.Features.EquipmentCategoryLinks;
using Himapp.PM.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Himapp.PM.Application.Controllers;

/// <summary>
/// Manual mapping of an equipment category with its masters. {kind} is one of:
/// sub-parts, work-descriptions, breakdown-reasons, pm-points, checklist-points, specifications.
/// </summary>
[ApiController]
[Authorize]
[Route("v1/pm/dynamics-equipment-categories/{categoryId:int}")]
public sealed class EquipmentCategoryLinksController : ControllerBase
{
    private const int MaxRemarksLength = 500;
    private readonly IEquipmentCategoryLinkService _service;
    public EquipmentCategoryLinksController(IEquipmentCategoryLinkService service) => _service = service;

    /// <summary>The "Configuration points n/6 added" panel.</summary>
    [HttpGet("configuration")]
    public Task<IActionResult> GetConfiguration(int categoryId, CancellationToken ct) =>
        Run(async () => Ok(await _service.GetConfigurationAsync(categoryId, ct)));

    /// <summary>Linked masters of one tab (removed links are not returned; each item carries its own isActive).</summary>
    [HttpGet("{kind}")]
    public Task<IActionResult> Get(int categoryId, string kind, CancellationToken ct) =>
        Run(async () => Ok(await _service.GetAsync(categoryId, kind, ct)));

    /// <summary>Items for the "Select ..." drop-down: active masters that are not linked to this category yet.</summary>
    [HttpGet("{kind}/available")]
    public Task<IActionResult> GetAvailable(int categoryId, string kind, CancellationToken ct) =>
        Run(async () => Ok(await _service.GetAvailableAsync(categoryId, kind, ct)));

    /// <summary>Links one master to the category.</summary>
    [HttpPost("{kind}")]
    public Task<IActionResult> Add(int categoryId, string kind, [FromBody] EquipmentCategoryLinkRequest request, CancellationToken ct) =>
        Run(async () =>
        {
            var error = Validate(request);
            if (error is not null) return BadRequest(new { message = error });
            return Ok(await _service.AddAsync(categoryId, kind, request, ct));
        });

    /// <summary>Save button of a tab: the list sent becomes the linked list; any master missing from it is marked deleted.</summary>
    [HttpPut("{kind}")]
    public Task<IActionResult> SaveAll(int categoryId, string kind, [FromBody] EquipmentCategoryLinkSaveRequest request, CancellationToken ct) =>
        Run(async () =>
        {
            foreach (var item in request.Items ?? new())
            {
                var error = Validate(item);
                if (error is not null) return BadRequest(new { message = error });
            }
            return Ok(await _service.SaveAllAsync(categoryId, kind, request, ct));
        });

    /// <summary>Changes the remarks and/or IsActive of one link.</summary>
    [HttpPut("{kind}/{linkId:int}")]
    public Task<IActionResult> Update(int categoryId, string kind, int linkId, [FromBody] EquipmentCategoryLinkUpdateRequest request, CancellationToken ct) =>
        Run(async () =>
        {
            if (request.Remarks is { Length: > MaxRemarksLength })
                return BadRequest(new { message = $"Remarks can be at most {MaxRemarksLength} characters." });
            var result = await _service.UpdateAsync(categoryId, kind, linkId, request, ct);
            return result is null ? NotFound() : Ok(result);
        });

    /// <summary>Soft delete: the link is marked IsDeleted = true (IsActive is not touched), the row is not removed.</summary>
    [HttpDelete("{kind}/{linkId:int}")]
    public Task<IActionResult> Delete(int categoryId, string kind, int linkId, CancellationToken ct) =>
        Run(async () => await _service.DeleteAsync(categoryId, kind, linkId, ct) ? Ok() : NotFound());

    private static string? Validate(EquipmentCategoryLinkRequest r)
    {
        if (r.MasterId <= 0) return "MasterId is required.";
        if (r.Remarks is { Length: > MaxRemarksLength }) return $"Remarks can be at most {MaxRemarksLength} characters.";
        return null;
    }

    private async Task<IActionResult> Run(Func<Task<IActionResult>> action)
    {
        try
        {
            return await action();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
    }
}
