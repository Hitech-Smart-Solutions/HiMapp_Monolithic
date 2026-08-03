using System.ComponentModel.DataAnnotations;

namespace Himapp.Execution.Application.Features.SiteDailyProgress.Models;

public sealed class CreateSiteDailyProgressRequest
{
    [Required]
    public int ProjectId { get; set; }

    public DateTimeOffset? ReportDate { get; set; }

    public string? Remarks { get; set; }

    public List<SiteDailyProgressDetailRequest>? Details { get; set; }
}

public sealed class UpdateSiteDailyProgressRequest
{
    [Required]
    public int ProjectId { get; set; }

    public DateTimeOffset? ReportDate { get; set; }

    public string? Remarks { get; set; }

    public List<SiteDailyProgressDetailRequest>? Details { get; set; }
}

public sealed class SiteDailyProgressModel
{
    public int Id { get; init; }
    public int ProgramId { get; init; }
    public DateOnly ReportDate { get; init; }
    public string? Remarks { get; init; }
    public bool IsActive { get; init; }
    public int? CreatedBy { get; init; }
    public DateTimeOffset CreatedDate { get; init; }
    public int? LastModifiedBy { get; init; }
    public DateTimeOffset LastModifiedDate { get; init; }
    public IReadOnlyCollection<SiteDailyProgressDetailModel> Details { get; init; }

    public SiteDailyProgressModel(int id, int programId, DateOnly reportDate, string? remarks, bool isActive, int? createdBy, DateTimeOffset createdDate, int? lastModifiedBy, DateTimeOffset lastModifiedDate, IReadOnlyCollection<SiteDailyProgressDetailModel> details)
    {
        Id = id;
        ProgramId = programId;
        ReportDate = reportDate;
        Remarks = remarks;
        IsActive = isActive;
        CreatedBy = createdBy;
        CreatedDate = createdDate;
        LastModifiedBy = lastModifiedBy;
        LastModifiedDate = lastModifiedDate;
        Details = details ?? Array.Empty<SiteDailyProgressDetailModel>();
    }
}

public sealed class SiteDailyProgressDto
{
    public int Id { get; set; }
    public int ProgramId { get; set; }
}
