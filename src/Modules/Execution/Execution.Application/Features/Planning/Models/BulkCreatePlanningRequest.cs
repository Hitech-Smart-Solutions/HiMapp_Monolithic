using Microsoft.AspNetCore.Http;

namespace Himapp.Execution.Application.Features.Planning.Models;

public sealed class BulkCreatePlanningRequest
{
    public int ProjectId { get; init; }
    public int PlanTypeID { get; init; }
    public DateOnly StartDate { get; init; }
    public DateOnly? EndDate { get; init; }
    public string? Remarks { get; init; }
    public int CreatedBy { get; init; }
    public IFormFile? ExcelFile { get; init; }
    public IFormFile? Attachment { get; init; }
}
