namespace Himapp.Workflow.Contracts.Models;

public sealed class AwaitingDepartmentalLabourSlipModel
{
    public int ProgramRowID { get; set; }

    public int EntityID { get; set; }

    public string? TransactionCode { get; set; }

    public DateTimeOffset? TransactionDate { get; set; }

    public string? ProgramName { get; set; }

    public string? ProjectName { get; set; }

    public short StatusID { get; set; }

    public string? StatusName { get; set; }

    public string? CreatedBy { get; set; }

    public string? PendingApprovalFor { get; set; }

    public string? SlipNo { get; set; }

    public string? PartyName { get; set; }

    public int? PartyID { get; set; }

    public int ProjectID { get; set; }

    public string? DocumentName { get; set; }

    public string? DocumentPath { get; set; }

    public string? DocumentContentType { get; set; }

    public long IsDisapprove { get; set; }

    public bool IsReference { get; set; }

    public short ApprovalLevel { get; set; }
}