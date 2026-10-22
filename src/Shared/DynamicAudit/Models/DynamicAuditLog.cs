using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Himapp.DynamicAudit.Models;

/// <summary>
/// Stores one audit entry per MediatR request decorated with <see cref="AuditLogAttribute"/>.
/// </summary>
[Table("DynamicAuditLog")]
public sealed class DynamicAuditLog
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    /// <summary>HTTP TraceIdentifier for correlating with request logs.</summary>
    public string TraceId { get; set; } = string.Empty;

    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;

    /// <summary>Name of the MediatR request type (e.g. "CreateProjectCommand").</summary>
    public string RequestName { get; set; } = string.Empty;

    /// <summary>JSON-serialized request payload.</summary>
    public string? RequestPayload { get; set; }

    /// <summary>"SUCCESS" or "FAILED"</summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>Exception message when Status = "FAILED".</summary>
    public string? ErrorMessage { get; set; }

    /// <summary>Handler execution time in milliseconds.</summary>
    public long DurationMs { get; set; }

    public DateTime Timestamp { get; set; }
}
