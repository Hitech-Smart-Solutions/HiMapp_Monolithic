using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Himapp.Execution.Application.Features.DailyProgress.Models;

public sealed class DailyProgressForApprovalSectionActivityModel
{
    [JsonPropertyName("ID")]
    public int ID { get; init; }

    [JsonPropertyName("SectionID")]
    public int SectionID { get; init; }

    [JsonPropertyName("SectionName")]
    public string? SectionName { get; init; }

    [JsonPropertyName("ActivityID")]
    public int ActivityID { get; init; }

    [JsonPropertyName("ActivityName")]
    public string? ActivityName { get; init; }

    [JsonPropertyName("UOMID")]
    public int UOMID { get; init; }

    [JsonPropertyName("UOMShortName")]
    public string? UOMShortName { get; init; }

    [JsonPropertyName("ActualQty")]
    public decimal ActualQty { get; init; }

    [JsonPropertyName("PlannedQty")]
    public decimal PlannedQty { get; init; }

    [JsonPropertyName("Rate")]
    public decimal Rate { get; init; }

    [JsonPropertyName("ActualAmount")]
    public decimal ActualAmount { get; init; }

    [JsonPropertyName("Variance")]
    public decimal? Variance { get; init; }

    [JsonPropertyName("Remarks")]
    public string? Remarks { get; init; }
}
