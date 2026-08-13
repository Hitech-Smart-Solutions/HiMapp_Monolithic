using System.Collections.Generic;

namespace Himapp.Execution.Contracts.References;

public interface IProjectLookup
{
    Task<ProjectMasterDto?> FindByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<IEnumerable<ProjectMasterDto>> GetAllAsync(CancellationToken cancellationToken = default);
}
