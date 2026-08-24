using System;
using System.Collections.Generic;
using System.Text;

namespace Himapp.Execution.Application.Features.DailyProgress.Models;

public sealed class DailyProgressPhotoModel
{
    public int Id { get; init; }
    public Guid UniqueId { get; init; }
    public string? FileName { get; set; }
    public string? FileType { get; set; }
    public int? FileSize { get; set; }
    public string? PhotoUrl { get; set; }
    public string? Caption { get; set; }

    public DailyProgressPhotoModel(int id, Guid uniqueId, string? fileName, string? fileType, int? fileSize, string? photoUrl, string? caption)
    {
        Id = id;
        UniqueId = uniqueId;
        FileName = fileName;
        FileType = fileType;
        FileSize = fileSize;
        PhotoUrl = photoUrl;
        Caption = caption;
    }
}

public sealed class DailyProgressPhotoRequest
{
    public string? FileName { get; set; }
    public string? FileType { get; set; }
    public int? FileSize { get; set; }
    public string? PhotoUrl { get; set; }
    public string? Caption { get; set; }
}
