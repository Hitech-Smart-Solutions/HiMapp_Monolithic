namespace Himapp.Workflow.Application.Features.CentralUserRoleMapping.Models;

public sealed record CentralUserRoleMappingDto(
    int Id,
    Guid UniqueId,
    string RoleCode,
    string? RoleName,
    int? StatusId,
    bool IsActive,
    int CreatedBy,
    DateTime CreatedDate,
    int LastModifiedBy,
    DateTime LastModifiedDate,
    IReadOnlyCollection<CentralUserRoleMappingDetailDto> Details);
