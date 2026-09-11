using System;
using System.Collections.Generic;
using System.Text;

namespace Himapp.Execution.Application.Features.DailyProgress.Models;

public sealed class DailyProgressHindranceModel
{
    public int Id { get; init; }
    public Guid UniqueId { get; init; }
    public int SectionId { get; init; }
    public string? Hindrance { get; set; }
    public string? AudioUrl { get; set; }

    public DailyProgressHindranceModel(int id, Guid uniqueId, int sectionId, string? hindrance, string? audioUrl)
    {
        Id = id;
        UniqueId = uniqueId;
        SectionId = sectionId;
        Hindrance = hindrance;
        AudioUrl = audioUrl;
    }
}

public sealed class DailyProgressHindranceRequest
{
    public int SectionId { get; set; }
    public string? Hindrance { get; set; }
    public string? AudioUrl { get; set; }
}
