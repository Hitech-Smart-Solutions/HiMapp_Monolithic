using Himapp.PM.Domain.Constants;
using Himapp.PM.Domain.Entities;
using Himapp.Shared.Responses.PM;
using Himapp.SharedKernel.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Himapp.PM.Application.Services;

/// <summary>Thrown when another run on the same data is already queued or running.</summary>
public sealed class SyncAlreadyRunningException : InvalidOperationException
{
    public SyncAlreadyRunningException(string entityName, string syncMode, string status, string startedBy, DateTime startedAtUtc)
        : base($"A {syncMode} sync for {entityName} is already {(status == DynamicsSyncStatus.Queued ? "queued" : "running")} " +
               $"(started by {startedBy} at {startedAtUtc:yyyy-MM-dd HH:mm:ss} UTC). Please try again after it finishes.")
    {
    }
}

/// <summary>A started run: the log row id and the user who started it (CreatedBy of the log row).</summary>
public sealed record DynamicsSyncRun(int LogId, int UserId);

public interface IDynamicsSyncLogService
{
    /// <summary>Creates a Queued row for a background job (checks that nothing is already queued or running). Returns the log id.</summary>
    Task<int> QueueAsync(string entityName, string syncMode, CancellationToken cancellationToken = default);

    Task SetJobIdAsync(int logId, string jobId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks the run as Running and returns the log id and the user who started it. Without a logId a new row is created (after the "already running" check).
    /// Returns null when the run was already completed (for example a repeated background job) and nothing should be done.
    /// </summary>
    Task<DynamicsSyncRun?> BeginAsync(string entityName, string syncMode, int? logId, CancellationToken cancellationToken = default);

    Task CompleteAsync(int logId, SyncResultDTO result, CancellationToken cancellationToken = default);

    Task CompleteAsync(int logId, string status, int recordsReset = 0, string? message = null, string? errorDetails = null, CancellationToken cancellationToken = default);
}

public sealed class DynamicsSyncLogService : IDynamicsSyncLogService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ICurrentUser _currentUser;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<DynamicsSyncLogService> _logger;

    public DynamicsSyncLogService(
        IServiceScopeFactory scopeFactory,
        ICurrentUser currentUser,
        IHttpContextAccessor httpContextAccessor,
        ILogger<DynamicsSyncLogService> logger)
    {
        _scopeFactory = scopeFactory;
        _currentUser = currentUser;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public async Task<int> QueueAsync(string entityName, string syncMode, CancellationToken cancellationToken = default)
    {
        var run = await CreateRunAsync(entityName, syncMode, DynamicsSyncStatus.Queued, DynamicsSyncTrigger.BackgroundJob, cancellationToken);
        return run.LogId;
    }

    public async Task SetJobIdAsync(int logId, string jobId, CancellationToken cancellationToken = default)
    {
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<IPMDbContext>();
            var row = await db.Set<DynamicsSyncLog>().FirstOrDefaultAsync(x => x.ID == logId, cancellationToken);
            if (row is null) return;

            row.JobId = jobId;
            row.LastModifiedBy = row.CreatedBy;
            row.LastModifiedDate = DateTime.UtcNow;
            await db.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Could not store job id {JobId} on sync log {LogId}", jobId, logId);
        }
    }

    public async Task<DynamicsSyncRun?> BeginAsync(string entityName, string syncMode, int? logId, CancellationToken cancellationToken = default)
    {
        if (logId is null)
        {
            // A direct call: from the API (a user is logged in) or from a background job that has no queue row (shown as System).
            var source = _httpContextAccessor.HttpContext is null ? DynamicsSyncTrigger.BackgroundJob : DynamicsSyncTrigger.Manual;
            return await CreateRunAsync(entityName, syncMode, DynamicsSyncStatus.Running, source, cancellationToken);
        }

        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<IPMDbContext>();
        var row = await db.Set<DynamicsSyncLog>().FirstOrDefaultAsync(x => x.ID == logId.Value, cancellationToken);

        if (row is null)
            return await CreateRunAsync(entityName, syncMode, DynamicsSyncStatus.Running, DynamicsSyncTrigger.BackgroundJob, cancellationToken);

        if (row.Status is DynamicsSyncStatus.Success or DynamicsSyncStatus.NothingToSync)
            return null; // already done (for example the job was repeated)

        var now = DateTime.UtcNow;
        row.Status = DynamicsSyncStatus.Running;
        row.StartedAt = now;
        row.CompletedAt = null;
        row.Message = null;
        row.ErrorDetails = null;
        row.LastModifiedBy = row.CreatedBy;
        row.LastModifiedDate = now;
        await db.SaveChangesAsync(cancellationToken);
        return new DynamicsSyncRun(row.ID, row.CreatedBy);
    }

    public Task CompleteAsync(int logId, SyncResultDTO result, CancellationToken cancellationToken = default)
    {
        var status = !result.Success ? DynamicsSyncStatus.Failed
                   : result.Fetched == 0 ? DynamicsSyncStatus.NothingToSync
                   : DynamicsSyncStatus.Success;

        return CompleteCoreAsync(logId, status, result.Fetched, result.Inserted, result.Updated, 0,
            result.Message, result.Success ? null : result.Message, cancellationToken);
    }

    public Task CompleteAsync(int logId, string status, int recordsReset = 0, string? message = null, string? errorDetails = null, CancellationToken cancellationToken = default) =>
        CompleteCoreAsync(logId, status, 0, 0, 0, recordsReset, message, errorDetails, cancellationToken);

    // ------------------------------------------------------------------

    private async Task CompleteCoreAsync(int logId, string status, int fetched, int inserted, int updated, int reset,
        string? message, string? errorDetails, CancellationToken cancellationToken)
    {
        // Own scope/DbContext: a failed sync must not stop the log from being saved. A log problem must never break the sync result.
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<IPMDbContext>();
            var row = await db.Set<DynamicsSyncLog>().FirstOrDefaultAsync(x => x.ID == logId, cancellationToken);
            if (row is null) return;

            var now = DateTime.UtcNow;
            row.Status = status;
            row.CompletedAt = now;
            row.RecordsFetched = fetched;
            row.RecordsInserted = inserted;
            row.RecordsUpdated = updated;
            row.RecordsReset = reset;
            row.Message = message;
            row.ErrorDetails = errorDetails;
            row.LastModifiedBy = row.CreatedBy;
            row.LastModifiedDate = now;
            await db.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Could not complete sync log {LogId} with status {Status}", logId, status);
        }
    }

    private async Task<DynamicsSyncRun> CreateRunAsync(string entityName, string syncMode, string status, string triggerSource, CancellationToken cancellationToken)
    {
        var userId = ResolveCallerUserId();

        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<IPMDbContext>();

        var busy = await db.Set<DynamicsSyncLog>().AsNoTracking()
            .Where(x => x.EntityName == entityName
                        && (x.Status == DynamicsSyncStatus.Queued || x.Status == DynamicsSyncStatus.Running))
            .OrderByDescending(x => x.StartedAt)
            .FirstOrDefaultAsync(cancellationToken);

        if (busy is not null)
        {
            throw new SyncAlreadyRunningException(
                busy.EntityName, busy.SyncMode, busy.Status,
                $"user {busy.CreatedBy}",
                busy.StartedAt);
        }

        var now = DateTime.UtcNow;
        var row = new DynamicsSyncLog
        {
            UniqueID = Guid.NewGuid(),
            EntityName = entityName,
            SyncMode = syncMode,
            TriggerSource = triggerSource,
            Status = status,
            StartedAt = now,
            CreatedBy = userId,
            CreatedDate = now,
            LastModifiedBy = userId,
            LastModifiedDate = now
        };

        db.Set<DynamicsSyncLog>().Add(row);
        await db.SaveChangesAsync(cancellationToken);
        return new DynamicsSyncRun(row.ID, userId);
    }

    /// <summary>
    /// The logged-in user (from the token). A run that has no web request at all (an old parameterless background job)
    /// has no user and is stored with user id 0.
    /// </summary>
    private int ResolveCallerUserId()
    {
        if (_httpContextAccessor.HttpContext is null)
            return 0;

        return _currentUser.UserId ?? throw new UnauthorizedAccessException("An authenticated user is required.");
    }
}
