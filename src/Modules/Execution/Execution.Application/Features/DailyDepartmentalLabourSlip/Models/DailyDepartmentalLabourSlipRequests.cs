using System.ComponentModel.DataAnnotations;

namespace Himapp.Execution.Application.Features.DailyDepartmentalLabourSlip.Models;

public sealed class CreateDailyDepartmentalLabourSlipRequest
{
    [Required]
    public int ProjectId { get; set; }

    public DateTimeOffset? SlipDate { get; set; }

    public string? Remarks { get; set; }
}

public sealed class UpdateDailyDepartmentalLabourSlipRequest
{
    [Required]
    public int ProjectId { get; set; }

    public DateTimeOffset? SlipDate { get; set; }

    public string? Remarks { get; set; }
}

public sealed class DailyDepartmentalLabourSlipDto
{
    public int Id { get; set; }
    public int ProgramId { get; set; }
}
