using Himapp.Files.Models;

namespace Himapp.Files.Services;

public sealed class InMemoryFileService : IFileService
{
    public Task<FileAsset> RegisterAsync(string fileName, string contentType, string purpose, long sizeBytes, CancellationToken cancellationToken = default)
    {
        var file = new FileAsset
        {
            FileName = fileName,
            ContentType = contentType,
            Purpose = purpose,
            SizeBytes = sizeBytes,
            StorageKey = $"local/{Guid.NewGuid():N}/{fileName}"
        };

        return Task.FromResult(file);
    }
}
