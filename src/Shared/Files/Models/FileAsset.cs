using Himapp.SharedKernel.Abstractions;

namespace Himapp.Files.Models;

public sealed class FileAsset : BaseEntity
{
    public string FileName { get; init; } = string.Empty;
    public string ContentType { get; init; } = string.Empty;
    public string StorageKey { get; init; } = string.Empty;
    public int SizeBytes { get; init; }
    public string Purpose { get; init; } = string.Empty;
}
