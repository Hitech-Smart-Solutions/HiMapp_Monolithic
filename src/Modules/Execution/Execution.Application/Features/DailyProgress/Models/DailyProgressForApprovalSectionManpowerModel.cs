using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Himapp.Execution.Application.Features.DailyProgress.Models;

public sealed class DailyProgressForApprovalSectionManpowerModel
{
    [JsonPropertyName("SectionID")]
    public int SectionID { get; init; }

    [JsonPropertyName("SectionName")]
    public string? SectionName { get; init; }

    [JsonPropertyName("PlannedManpower")]
    public decimal PlannedManpower { get; init; }

    [JsonPropertyName("ActualManpower")]
    public decimal ActualManpower { get; init; }

    [JsonPropertyName("Remarks")]
    public string? Remarks { get; init; }
}
