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

    [JsonPropertyName("ContractorID")]
    public int? ContractorID { get; init; }

    [JsonPropertyName("ContractorName")]
    public string? ContractorName { get; init; }

    [JsonPropertyName("Skilled")]
    public decimal Skilled { get; init; }

    [JsonPropertyName("UnSkilled")]
    public decimal UnSkilled { get; init; }

    [JsonPropertyName("Other")]
    public decimal Other { get; init; }

    [JsonPropertyName("PlannedManpower")]
    public decimal PlannedManpower { get; init; }

    [JsonPropertyName("ActualManpower")]
    public decimal ActualManpower { get; init; }

    [JsonPropertyName("Remarks")]
    public string? Remarks { get; init; }
}
