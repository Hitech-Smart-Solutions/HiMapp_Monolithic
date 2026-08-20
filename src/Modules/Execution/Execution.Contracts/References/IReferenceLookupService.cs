namespace Himapp.Execution.Contracts.References;

/// <summary>
/// Aggregate read-only lookup service for external reference data used by the Execution module.
/// Implementations should be provided by the Infrastructure layer and resolve data from the
/// owning microservice (public schema) — e.g. via HTTP or shared database.
/// This interface keeps the Contracts project focused on abstractions and DTOs only.
/// </summary>
public interface IReferenceLookupService
{
    Task<ProjectMasterDto?> GetProjectAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<ProjectMasterDto>> GetProjectsAsync(CancellationToken cancellationToken = default);

    Task<UomDto?> GetUomAsync(long id, CancellationToken cancellationToken = default);
    Task<IEnumerable<UomDto>> GetUomsAsync(CancellationToken cancellationToken = default);

    Task<ProjectLocationMasterDto?> GetProjectLocationAsync(long id, CancellationToken cancellationToken = default);
    Task<IEnumerable<ProjectLocationMasterDto>> GetProjectLocationsByProjectIdAsync(long projectId, CancellationToken cancellationToken = default);
}
