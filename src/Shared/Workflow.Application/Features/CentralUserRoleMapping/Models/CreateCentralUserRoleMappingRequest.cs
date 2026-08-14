namespace Himapp.Workflow.Application.Features.CentralUserRoleMapping.Models;

public sealed record CreateCentralUserRoleMappingRequest(
    string RoleCode,
    string? RoleName,
    int? StatusId,
    int CreatedBy,
    List<CentralUserRoleMappingDetailRequest>? Details = null);
