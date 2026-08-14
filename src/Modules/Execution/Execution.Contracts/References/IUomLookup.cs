using System.Collections.Generic;

namespace Himapp.Execution.Contracts.References;

public interface IUomLookup
{
    Task<UomDto?> FindByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<IEnumerable<UomDto>> GetAllAsync(CancellationToken cancellationToken = default);
}
