using Himapp.PM.Application.Features.EquipmentSetup.Models;
using Himapp.PM.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Himapp.PM.Application.Controllers;

/// <summary>
/// Equipment 360 setup wizard (8 steps). Every save endpoint takes ?draft=true for "Save draft" (step stays In progress)
/// and draft=false (default) for "Save and continue" (step becomes Complete).
/// </summary>
[ApiController]
[Authorize]
[Route("v1/pm/equipment-setup/{equipmentId:int}")]
public sealed class EquipmentSetupController : ControllerBase
{
    private readonly IEquipmentSetupService _service;
    public EquipmentSetupController(IEquipmentSetupService service) => _service = service;

    /// <summary>Step list with status (InProgress / Complete / Locked) for the left side of the wizard.</summary>
    [HttpGet]
    public Task<IActionResult> Overview(int equipmentId, CancellationToken ct) =>
        Run(async () => Ok(await _service.GetOverviewAsync(equipmentId, ct)));

    // Step 1
    [HttpGet("details")]
    public Task<IActionResult> GetDetails(int equipmentId, CancellationToken ct) =>
        Run(async () => Ok(await _service.GetDetailsAsync(equipmentId, ct)));

    [HttpPost("details/confirm")]
    public Task<IActionResult> ConfirmDetails(int equipmentId, CancellationToken ct) =>
        Run(async () => Ok(await _service.ConfirmDetailsAsync(equipmentId, ct)));

    // Step 2
    [HttpGet("information")]
    public Task<IActionResult> GetInformation(int equipmentId, CancellationToken ct) =>
        Run(async () => Ok(await _service.GetInformationAsync(equipmentId, ct)));

    [HttpPut("information")]
    public Task<IActionResult> SaveInformation(int equipmentId, [FromBody] EquipmentInformationRequest request, [FromQuery] bool draft = false, CancellationToken ct = default) =>
        Run(async () => Ok(await _service.SaveInformationAsync(equipmentId, request, draft, ct)));

    // Step 3
    [HttpGet("meter-fuel")]
    public Task<IActionResult> GetMeterFuel(int equipmentId, CancellationToken ct) =>
        Run(async () => Ok(await _service.GetMeterFuelAsync(equipmentId, ct)));

    [HttpPut("meter-fuel")]
    public Task<IActionResult> SaveMeterFuel(int equipmentId, [FromBody] EquipmentMeterFuelRequest request, [FromQuery] bool draft = false, CancellationToken ct = default) =>
        Run(async () => Ok(await _service.SaveMeterFuelAsync(equipmentId, request, draft, ct)));

    // Step 4 (files are uploaded with the existing POST v1/upload/upload; only the returned names are sent here)
    [HttpGet("documents")]
    public Task<IActionResult> GetDocuments(int equipmentId, CancellationToken ct) =>
        Run(async () => Ok(await _service.GetDocumentsAsync(equipmentId, ct)));

    [HttpPut("documents")]
    public Task<IActionResult> SaveDocuments(int equipmentId, [FromBody] EquipmentDocumentsSaveRequest request, [FromQuery] bool draft = false, CancellationToken ct = default) =>
        Run(async () => Ok(await _service.SaveDocumentsAsync(equipmentId, request, draft, ct)));

    // Step 5
    [HttpGet("pm")]
    public Task<IActionResult> GetPm(int equipmentId, CancellationToken ct) =>
        Run(async () => Ok(await _service.GetPmAsync(equipmentId, ct)));

    [HttpGet("pm/available")]
    public Task<IActionResult> GetPmAvailable(int equipmentId, CancellationToken ct) =>
        Run(async () => Ok(await _service.GetPmAvailableAsync(equipmentId, ct)));

    [HttpPut("pm")]
    public Task<IActionResult> SavePm(int equipmentId, [FromBody] EquipmentPmSaveRequest request, [FromQuery] bool draft = false, CancellationToken ct = default) =>
        Run(async () => Ok(await _service.SavePmAsync(equipmentId, request, draft, ct)));

    /// <summary>"Restore defaults": category-suggested points go back to the category's trigger values.</summary>
    [HttpPost("pm/restore-defaults")]
    public Task<IActionResult> RestorePmDefaults(int equipmentId, CancellationToken ct) =>
        Run(async () => Ok(await _service.RestorePmDefaultsAsync(equipmentId, ct)));

    // Step 6
    [HttpGet("specifications")]
    public Task<IActionResult> GetSpecifications(int equipmentId, CancellationToken ct) =>
        Run(async () => Ok(await _service.GetSpecificationsAsync(equipmentId, ct)));

    [HttpGet("specifications/available")]
    public Task<IActionResult> GetSpecificationsAvailable(int equipmentId, CancellationToken ct) =>
        Run(async () => Ok(await _service.GetSpecificationsAvailableAsync(equipmentId, ct)));

    [HttpPut("specifications")]
    public Task<IActionResult> SaveSpecifications(int equipmentId, [FromBody] EquipmentSpecificationsSaveRequest request, [FromQuery] bool draft = false, CancellationToken ct = default) =>
        Run(async () => Ok(await _service.SaveSpecificationsAsync(equipmentId, request, draft, ct)));

    // Steps 7 and 8
    [HttpGet("review")]
    public Task<IActionResult> Review(int equipmentId, CancellationToken ct) =>
        Run(async () => Ok(await _service.GetReviewAsync(equipmentId, ct)));

    [HttpPost("complete")]
    public Task<IActionResult> Complete(int equipmentId, CancellationToken ct) =>
        Run(async () => Ok(await _service.CompleteAsync(equipmentId, ct)));

    [HttpGet("completion")]
    public Task<IActionResult> Completion(int equipmentId, CancellationToken ct) =>
        Run(async () => Ok(await _service.GetCompletionAsync(equipmentId, ct)));

    private async Task<IActionResult> Run(Func<Task<IActionResult>> action)
    {
        try
        {
            return await action();
        }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
        catch (ArgumentException ex) { return BadRequest(new { message = ex.Message }); }
        catch (InvalidOperationException ex) { return Conflict(new { message = ex.Message }); }
        catch (UnauthorizedAccessException) { return Unauthorized(); }
    }
}
