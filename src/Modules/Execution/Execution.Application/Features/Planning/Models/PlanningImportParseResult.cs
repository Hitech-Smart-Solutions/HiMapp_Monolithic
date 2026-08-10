using System.Collections.Generic;

namespace Himapp.Execution.Application.Features.Planning.Models;

public sealed class PlanningImportParseResult
{
    public List<PlanningDetailRequest> Details { get; init; } = new List<PlanningDetailRequest>();
    public List<string> Errors { get; init; } = new List<string>();

    public PlanningImportParseResult() { }
    public PlanningImportParseResult(List<PlanningDetailRequest> details, List<string> errors)
    {
        Details = details;
        Errors = errors;
    }
}
