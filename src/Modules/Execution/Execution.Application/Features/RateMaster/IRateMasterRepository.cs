using Himapp.Execution.Application.Features.RateMaster.Models;

namespace Himapp.Execution.Application.Features.RateMaster;

internal interface IRateMasterRepository
{
    Task<IReadOnlyCollection<RateMasterModel>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<RateMasterModel?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<RateMasterModel> AddAsync(CreateRateMasterRequest model, CancellationToken cancellationToken = default);
    Task<RateMasterModel?> UpdateAsync(long id, UpdateRateMasterRequest model, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(long id, CancellationToken cancellationToken = default);
}
