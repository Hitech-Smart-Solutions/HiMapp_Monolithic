namespace Himapp.Execution.Application.Features.ExecutionProjectConfigs.Models;

public sealed record ExecutionProjectConfigModel(
    int Id,
    Guid UniqueId,
    int ProjectId,
    decimal MaxHours,
    int DDLSBackDatedDays,
    bool DDLSDPRLinkage,
    bool DPREntryContinue,
    bool PlanningRequired,
    bool IsActive,
    int CreatedBy,
    DateTimeOffset CreatedDate,
    int LastModifiedBy,
    DateTimeOffset LastModifiedDate);

public sealed record CreateExecutionProjectConfigRequest(
    int ProjectId,
    decimal MaxHours,
    int DDLSBackDatedDays,
    bool DDLSDPRLinkage,
    bool DPREntryContinue,
    bool PlanningRequired,
    int CreatedBy);

public sealed record UpdateExecutionProjectConfigRequest(
    int ProjectId,
    decimal MaxHours,
    int DDLSBackDatedDays,
    bool DDLSDPRLinkage,
    bool DPREntryContinue,
    bool PlanningRequired,
    bool IsActive,
    int LastModifiedBy);
