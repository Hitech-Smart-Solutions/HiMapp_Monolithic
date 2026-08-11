using System.ComponentModel.DataAnnotations;

namespace Himapp.Execution.Application.Features.DailyLabor.Models;

public sealed class DailyLaborDetailRequest
{
    public int? ContractorId { get; set; }
    public int? CategoryId { get; set; }
    public int? Skilled { get; set; }
    public int? UnSkilled { get; set; }
    public string? Remarks { get; set; }
    public int? Mat { get; set; }
    public string? ContractorName { get; set; }
    public int? ActivityId { get; set; }
}

public sealed class CreateDailyLaborRequest
{
    [Required]
    public int ProjectId { get; set; }

    [Required]
    public DateTime ReportDate { get; set; }

    public string? Remarks { get; set; }

    /// <summary>
    /// Status / State identifier. Use meaningful defaults in caller (e.g. Draft = 1).
    /// </summary>
    public int Status { get; set; } = 1;

    public List<DailyLaborDetailRequest>? Details { get; set; }
}

public sealed class UpdateDailyLaborRequest
{
    [Required]
    public int ProjectId { get; set; }

    [Required]
    public DateTime ReportDate { get; set; }

    public string? Remarks { get; set; }

    public int Status { get; set; } = 1;

    public List<DailyLaborDetailRequest>? Details { get; set; }
}
