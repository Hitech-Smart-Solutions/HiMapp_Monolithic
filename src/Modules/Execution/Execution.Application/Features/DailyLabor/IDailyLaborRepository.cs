using Himapp.Execution.Application.Features.DailyLabor.Models;

namespace Himapp.Execution.Application.Features.DailyLabor;

internal interface IDailyLaborRepository
{
    Task<IReadOnlyCollection<Models.DailyLaborModel>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Models.DailyLaborModel?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<Models.DailyLaborModel> AddAsync(Models.CreateDailyLaborRequest request, CancellationToken cancellationToken = default);
    Task<Models.DailyLaborModel?> UpdateAsync(long id, Models.UpdateDailyLaborRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(long id, CancellationToken cancellationToken = default);
}
