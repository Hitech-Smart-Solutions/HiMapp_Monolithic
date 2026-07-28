using Himapp.Audit.Models;

namespace Himapp.Audit.Abstractions;

/// <summary>
/// Service for queueing user action audit log entries asynchronously.
/// </summary>
public interface IAuditService
{
    /// <summary>
    /// Queue a user action log entry for async background processing.
    /// Returns immediately — the actual DB write happens in the background consumer.
    /// </summary>
    /// <param name="userId">The user who performed the action.</param>
    /// <param name="actionId">The action type ID (from <see cref="Actions"/> enum).</param>
    /// <param name="programId">The program/project ID the action belongs to.</param>
    /// <param name="programRowId">The ID of the affected entity/record.</param>
    /// <param name="programRowCode">Human-readable entity type name (e.g., "DailyLabor").</param>
    /// <param name="remarks">Optional remarks about the action.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    ValueTask LogAsync(
        int userId,
        int actionId,
        int programId,
        int programRowId,
        string? programRowCode,
        string? remarks,
        CancellationToken cancellationToken = default);
}

