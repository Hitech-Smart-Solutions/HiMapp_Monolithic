namespace Himapp.PM.Application.Features.DynamicsSyncLogs.Models;

public sealed class DynamicsSyncLogDto
{
    public int Id { get; set; }
    public Guid UniqueID { get; set; }
    public string EntityName { get; set; } = string.Empty;
    public string SyncMode { get; set; } = string.Empty;
    public string TriggerSource { get; set; } = string.Empty;
    public string? JobId { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public double? DurationSeconds { get; set; }
    public int RecordsFetched { get; set; }
    public int RecordsInserted { get; set; }
    public int RecordsUpdated { get; set; }
    public int RecordsReset { get; set; }
    public string? Message { get; set; }
    public string? ErrorDetails { get; set; }
    /// <summary>User who started the run.</summary>
    public int CreatedBy { get; set; }
    public DateTime CreatedDate { get; set; }
    public int LastModifiedBy { get; set; }
    public DateTime LastModifiedDate { get; set; }
}
