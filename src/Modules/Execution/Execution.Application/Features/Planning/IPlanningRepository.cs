using Himapp.Execution.Application.Features.Planning.Models;

namespace Himapp.Execution.Application.Features.Planning;

internal interface IPlanningRepository
{
    Task<IReadOnlyCollection<Models.PlanningModel>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Models.PlanningModel?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<Models.PlanningModel> AddAsync(Models.CreatePlanningRequest request, CancellationToken cancellationToken = default);
    Task<Models.PlanningModel?> UpdateAsync(long id, Models.UpdatePlanningRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(long id, CancellationToken cancellationToken = default);
}
