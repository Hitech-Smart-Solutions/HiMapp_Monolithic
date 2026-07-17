using Himapp.Execution.Application.Features.DailyProgress.Models;

namespace Himapp.Execution.Application.Features.DailyProgress;

internal interface IDailyProgressRepository
{
    Task<IReadOnlyCollection<Models.DailyProgressModel>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Models.DailyProgressModel?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<Models.DailyProgressModel> AddAsync(Models.CreateDailyProgressRequest request, CancellationToken cancellationToken = default);
    Task<Models.DailyProgressModel?> UpdateAsync(long id, Models.UpdateDailyProgressRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(long id, CancellationToken cancellationToken = default);
}
