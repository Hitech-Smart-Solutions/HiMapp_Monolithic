using System;
using System.Collections.Generic;
using System.Text;

namespace Himapp.Execution.Application.Features.DailyProgress.Models;

public sealed class DailyProgressPhotoModel
{
    public int Id { get; init; }
    public Guid UniqueId { get; init; }
    public string PhotoUrl { get; set; } = string.Empty;
    public string? Caption { get; set; }

    public DailyProgressPhotoModel(int id, Guid uniqueId, string? photoUrl, string? caption)
    {
        Id = id;
        UniqueId = uniqueId;
        PhotoUrl = photoUrl ?? string.Empty;
        Caption = caption;
    }
}

public sealed class DailyProgressPhotoRequest
{
    public string PhotoUrl { get; set; } = string.Empty;
    public string? Caption { get; set; }
}
