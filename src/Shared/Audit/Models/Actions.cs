namespace Himapp.Audit.Models;

/// <summary>
/// System action types mapped to integer IDs for TransactionActionHistory.
/// </summary>
public enum Actions
{
    Inserted = 501,
    Updated = 502,
    Deleted = 503,
    Activated = 504,
    Inactivated = 505,
    Viewed = 506
}

