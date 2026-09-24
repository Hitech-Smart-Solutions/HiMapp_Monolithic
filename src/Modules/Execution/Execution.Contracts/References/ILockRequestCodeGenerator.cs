namespace Himapp.Execution.Contracts.References;

/// <summary>
/// Generates LockRequest codes for a given project. Implementations should ensure codes are
/// project-scoped and incremented based on the last stored RequestCode.
/// </summary>
public interface ILockRequestCodeGenerator
{
    Task<string> GenerateLockRequestCodeAsync(int projectId, CancellationToken cancellationToken = default);
}