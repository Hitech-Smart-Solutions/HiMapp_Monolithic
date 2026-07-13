namespace Himapp.Execution.Application.Features.Activities;

internal interface IActivityRepository
{
    Task<IReadOnlyCollection<ActivityDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<ActivityDto?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<ActivityDto> AddAsync(ActivityDto activity, CancellationToken cancellationToken = default);
    Task<ActivityDto?> UpdateAsync(ActivityDto activity, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(long id, CancellationToken cancellationToken = default);
}
