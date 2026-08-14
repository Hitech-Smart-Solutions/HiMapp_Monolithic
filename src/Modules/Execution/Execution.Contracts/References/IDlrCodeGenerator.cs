namespace Himapp.Execution.Contracts.References;

/// <summary>
/// Generates DLR codes for a given project. Implementations should ensure codes are
/// project-scoped and incremented based on the last stored DLR code.
/// </summary>
public interface IDlrCodeGenerator
{
    Task<string> GenerateDLRCodeAsync(int projectId, CancellationToken cancellationToken = default);
}
