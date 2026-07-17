using Himapp.Execution.Application.Features.Manpower.Models;

namespace Himapp.Execution.Application.Features.Manpower;

internal interface IManpowerRepository
{
    Task<IReadOnlyCollection<Models.ManpowerModel>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Models.ManpowerModel?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<Models.ManpowerModel> AddAsync(Models.CreateManpowerRequest request, CancellationToken cancellationToken = default);
    Task<Models.ManpowerModel?> UpdateAsync(long id, Models.UpdateManpowerRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(long id, CancellationToken cancellationToken = default);
}
