using System.ComponentModel.DataAnnotations;

namespace Himapp.Workflow.Models;

/// <summary>
/// Shared request DTO for performing a workflow action (approve, reject, etc.)
/// </summary>
public sealed class WorkflowActionRequest
{
    /// <summary>
    /// The action to perform: "Approve", "Reject", "Cancel", "Dispute", "Resolve"
    /// </summary>
    [Required]
    public string Action { get; set; } = string.Empty;

    /// <summary>
    /// Optional comment / reason for the action.
    /// </summary>
    public string? Comment { get; set; }
}

