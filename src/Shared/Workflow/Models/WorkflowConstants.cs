namespace Himapp.Workflow.Models;

/// <summary>
/// Centralised workflow action and state string constants.
/// </summary>
public static class WorkflowActions
{
    public const string Submit = "Submit";
    public const string Approve = "Approve";
    public const string Reject = "Reject";
    public const string Cancel = "Cancel";
    public const string Dispute = "Dispute";
    public const string Resolve = "Resolve";
}

public static class WorkflowStates
{
    public const string Draft = "Draft";
    public const string PendingApproval = "PendingApproval";
    public const string Approved = "Approved";
    public const string Rejected = "Rejected";
    public const string Cancelled = "Cancelled";
    public const string Disputed = "Disputed";
    public const string Resolved = "Resolved";
}

