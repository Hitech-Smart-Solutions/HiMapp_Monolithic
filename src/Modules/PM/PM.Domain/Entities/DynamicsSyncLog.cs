using Himapp.SharedKernel.Abstractions;

namespace Himapp.PM.Domain.Entities;

/// <summary>
/// One row per Dynamics sync / reset run: who ran it (CreatedBy), when, how many records were taken and what was done.
/// The Status column is also used to stop two runs on the same data at the same time (Queued / Running).
/// </summary>
public sealed class DynamicsSyncLog : BaseEntity
{
    public string EntityName { get; set; } = string.Empty;
    public string SyncMode { get; set; } = string.Empty;
    public string TriggerSource { get; set; } = string.Empty;
    public string? JobId { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public int RecordsFetched { get; set; }
    public int RecordsInserted { get; set; }
    public int RecordsUpdated { get; set; }
    public int RecordsReset { get; set; }
    public string? Message { get; set; }
    public string? ErrorDetails { get; set; }
}
