namespace Himapp.Workflow.Application.Features.CentralUserRoleMapping.Models;

public sealed record CentralUserRoleMappingDetailRequest(
    int UserId,
    int ProjectId,
    int? StatusId,
    bool IsActive = true);
