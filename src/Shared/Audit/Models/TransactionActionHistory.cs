using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Himapp.Audit.Models;

/// <summary>
/// Represents a Transaction Action History entity — used for system-wide user action audit logging.
/// This entity maps to the existing TransactionActionHistory table.
/// </summary>
[Table("TransactionActionHistory")]
public sealed class TransactionActionHistory
{
    /// <summary>
    /// Globally unique identifier for the record.
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid UniqueID { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Auto-incrementing identity column.
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    /// <summary>
    /// The user who performed the action (from ICurrentUser).
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// The action type ID (501 = Inserted, 502 = Updated, 503 = Deleted, 504 = Activated, 505 = Inactivated, 506 = Viewed).
    /// Maps to <see cref="Actions"/> enum.
    /// </summary>
    public int ActionId { get; set; }

    /// <summary>
    /// The program/project ID under which the action was performed.
    /// </summary>
    public int ProgramId { get; set; }

    /// <summary>
    /// The ID of the affected entity/record (e.g., the record that was created/updated/deleted).
    /// </summary>
    public int ProgramRowId { get; set; }

    /// <summary>
    /// A human-readable code/name for the entity type (e.g., "DailyLabor", "DailyProgress", "GatePass").
    /// Derived from the controller route or entity name.
    /// </summary>
    public string? ProgramRowCode { get; set; }

    /// <summary>
    /// Optional remarks lookup ID (default 0 for system-generated logs).
    /// </summary>
    public int RemarksId { get; set; }

    /// <summary>
    /// Optional free-text remarks about the action.
    /// </summary>
    public string Remarks { get; set; } = string.Empty;

    /// <summary>
    /// Whether this record is active (default true).
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// The user who created this record (same as UserId for system-generated logs).
    /// </summary>
    public int CreatedBy { get; set; }

    /// <summary>
    /// When this record was created.
    /// </summary>
    public DateTime CreatedDate { get; set; }

    /// <summary>
    /// The user who last modified this record (same as CreatedBy for inserts).
    /// </summary>
    public int LastModifiedBy { get; set; }

    /// <summary>
    /// When this record was last modified.
    /// </summary>
    public DateTime LastModifiedDate { get; set; }
}

