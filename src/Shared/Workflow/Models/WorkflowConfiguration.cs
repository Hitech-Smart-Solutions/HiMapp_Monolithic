namespace Himapp.Workflow.Models;

/// <summary>
/// Defines the approval levels / workflow configuration for each entity type.
/// This can be loaded from DB, appsettings, or hardcoded for now.
/// </summary>
public sealed record WorkflowLevel(int Level, string Name, string Role);

public sealed record WorkflowConfiguration(
    string EntityName,
    IReadOnlyCollection<WorkflowLevel> Levels,
    string StartState = "Draft",
    string SubmittedState = "PendingApproval",
    string ApprovedState = "Approved",
    string RejectedState = "Rejected");

/// <summary>
/// Default workflow configurations. Extend this as needed.
/// </summary>
public static class DefaultWorkflowConfigurations
{
    public static readonly IReadOnlyDictionary<string, WorkflowConfiguration> Defaults =
        new Dictionary<string, WorkflowConfiguration>
        {
            ["DailyLabor"] = new WorkflowConfiguration(
                EntityName: "DailyLabor",
                Levels: new[]
                {
                    new WorkflowLevel(1, "L1 - Supervisor", "Supervisor"),
                    new WorkflowLevel(2, "L2 - Manager", "Manager")
                }),
            ["DailyProgress"] = new WorkflowConfiguration(
                EntityName: "DailyProgress",
                Levels: new[]
                {
                    new WorkflowLevel(1, "L1 - Supervisor", "Supervisor"),
                    new WorkflowLevel(2, "L2 - Manager", "Manager")
                })
        };

    public static WorkflowConfiguration GetFor(string entityName) =>
        Defaults.TryGetValue(entityName, out var config)
            ? config
            : new WorkflowConfiguration(
                EntityName: entityName,
                Levels: new[] { new WorkflowLevel(1, "L1 - Default Approver", "Approver") });
}

