namespace Himapp.Execution.Application.Features.ProjectActivities.Models;

public sealed record ProjectActivityModel(
    int Id,
    System.Guid UniqueId,
    int ProjectId,
    int ActivityId,
    bool IsActive,
    bool Enabled, 
    decimal RevenueRate,
    decimal SkilledLabourRate,
    decimal UnSkilledLabourRate,
    decimal OtherLabourRate, 
    bool OutputRequired,
    int? CreatedBy,
    System.DateTimeOffset CreatedDate,
    int? LastModifiedBy,
    System.DateTimeOffset LastModifiedDate
);

public sealed record ProjectActivityRefrenceModel(
    int Id,
    int ProjectId,
    int ActivityId,
    string ActivityName,
    bool Enabled,
    decimal RevenueRate,
    decimal SkilledLabourRate,
    decimal UnSkilledLabourRate,
    decimal OtherLabourRate,
    bool OutputRequired,
    int UOMID,
    string UOMName
);

public sealed record CreateProjectActivityRequest(int ProjectId, int ActivityId, bool Enabled, decimal RevenueRate, decimal SkilledLabourRate, decimal UnSkilledLabourRate, decimal OtherLabourRate, bool OutputRequired, int? CreatedBy, int? LastModifiedBy);

public sealed record UpdateProjectActivityRequest(int Id, int ProjectId, int ActivityId, bool Enabled, decimal RevenueRate, decimal SkilledLabourRate, decimal UnSkilledLabourRate, decimal OtherLabourRate, bool OutputRequired, int? CreatedBy, int? LastModifiedBy);
