namespace Himapp.Workflow.Contracts.Models;

public sealed class AwaitingDailyProgressModel
{
    public int ProgramRowID { get; set; }

    public int EntityID { get; set; }

    public string? TransactionCode { get; set; }

    public DateTime? TransactionDate { get; set; }

    public string? ProgramName { get; set; }

    public string? ProjectName { get; set; }

    public short StatusID { get; set; }

    public string? StatusName { get; set; }

    public string? CreatedBy { get; set; }

    public string? PendingApprovalFor { get; set; }

    public string? DPRCode { get; set; }

    public int ProjectID { get; set; }

    public long IsDisapprove { get; set; }

    public bool IsReference { get; set; }

    public short ApprovalLevel { get; set; }
}