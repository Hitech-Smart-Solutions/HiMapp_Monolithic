using System.Collections.Generic;

namespace Himapp.Execution.Contracts.References;

public interface IProjectLocationLookup
{
    Task<ProjectLocationMasterDto?> FindByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<IEnumerable<ProjectLocationMasterDto>> GetAllByProjectIdAsync(long projectId, CancellationToken cancellationToken = default);
}
