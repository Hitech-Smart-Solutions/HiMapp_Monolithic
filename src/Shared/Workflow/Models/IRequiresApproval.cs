namespace Himapp.Workflow.Models;

/// <summary>
/// Marker interface that a response DTO implements to indicate
/// that this entity requires approval workflow.
/// The action filter reads this from successful controller responses.
/// </summary>
public interface IRequiresApproval
{
    /// <summary>
    /// Logical entity name used as the workflow type key (e.g. "DailyLabor", "DailyProgress").
    /// </summary>
    string EntityName { get; }

    /// <summary>
    /// The unique ID of the entity that was just created/updated.
    /// </summary>
    long EntityId { get; }
}

