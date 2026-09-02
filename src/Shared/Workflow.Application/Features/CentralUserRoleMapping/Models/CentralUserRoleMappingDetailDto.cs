namespace Himapp.Workflow.Application.Features.CentralUserRoleMapping.Models;

public sealed record CentralUserRoleMappingDetailDto(
    int Id,
    Guid UniqueId,
    int UserId,
    int ProjectId,
    int? StatusId,
    bool IsActive,
    int CreatedBy,
    DateTime CreatedDate,
    int LastModifiedBy,
    DateTime LastModifiedDate);
