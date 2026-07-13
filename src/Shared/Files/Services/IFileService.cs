using Himapp.Files.Models;

namespace Himapp.Files.Services;

public interface IFileService
{
    Task<FileAsset> RegisterAsync(string fileName, string contentType, string purpose, long sizeBytes, CancellationToken cancellationToken = default);
}
