namespace Himapp.Workflow.Application.Features.CentralUserRoleMapping.Models;

public sealed record RoleMappingWithWorkflowResult(
    bool IsLinked,
    string? WorkflowNames
);
