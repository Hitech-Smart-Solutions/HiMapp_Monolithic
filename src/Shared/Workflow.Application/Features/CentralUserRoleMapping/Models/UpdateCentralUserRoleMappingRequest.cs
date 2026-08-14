namespace Himapp.Workflow.Application.Features.CentralUserRoleMapping.Models;

public sealed record UpdateCentralUserRoleMappingRequest(
    string RoleCode,
    string? RoleName,
    int? StatusId,
    bool IsActive,
    int LastModifiedBy,
    List<CentralUserRoleMappingDetailRequest>? Details = null);
