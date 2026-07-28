using System.ComponentModel.DataAnnotations;

namespace Himapp.Execution.Application.Features.SiteDailyProgress.Models;

public sealed class CreateSiteDailyProgressRequest
{
    [Required]
    public int ProjectId { get; set; }

    public DateTimeOffset? ReportDate { get; set; }

    public string? Remarks { get; set; }
}

public sealed class UpdateSiteDailyProgressRequest
{
    [Required]
    public int ProjectId { get; set; }

    public DateTimeOffset? ReportDate { get; set; }

    public string? Remarks { get; set; }
}

public sealed class SiteDailyProgressDto
{
    public int Id { get; set; }
    public int ProgramId { get; set; }
}
