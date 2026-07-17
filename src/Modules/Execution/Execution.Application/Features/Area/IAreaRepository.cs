using Himapp.Execution.Application.Features.Area.Models;

namespace Himapp.Execution.Application.Features.Area;

internal interface IAreaRepository
{
    Task<IReadOnlyCollection<Models.AreaModel>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Models.AreaModel?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<Models.AreaModel> AddAsync(Models.CreateAreaRequest request, CancellationToken cancellationToken = default);
    Task<Models.AreaModel?> UpdateAsync(long id, Models.UpdateAreaRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(long id, CancellationToken cancellationToken = default);
}
