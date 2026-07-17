using Himapp.Execution.Application.Features.Uom.Models;

namespace Himapp.Execution.Application.Features.Uom;

internal interface IUomRepository
{
    Task<IReadOnlyCollection<Models.UomModel>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Models.UomModel?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<Models.UomModel> AddAsync(Models.CreateUomRequest request, CancellationToken cancellationToken = default);
    Task<Models.UomModel?> UpdateAsync(long id, Models.UpdateUomRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(long id, CancellationToken cancellationToken = default);
}
