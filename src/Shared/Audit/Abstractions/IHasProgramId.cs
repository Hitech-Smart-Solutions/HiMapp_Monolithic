namespace Himapp.Audit.Abstractions;

/// <summary>
/// Marker interface to extract ProgramId/ProjectId from request DTOs or response DTOs
/// for automatic audit logging.
/// 
/// Implement this interface on any DTO that carries a project/program identifier.
/// </summary>
public interface IHasProgramId
{
    /// <summary>
    /// The program or project identifier associated with this entity.
    /// </summary>
    long ProgramId { get; }
}

