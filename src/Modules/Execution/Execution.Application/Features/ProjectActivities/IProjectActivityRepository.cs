using Himapp.Execution.Application.Features.ProjectActivities.Models;

namespace Himapp.Execution.Application.Features.ProjectActivities;

internal interface IProjectActivityRepository
{
    Task<IReadOnlyCollection<ProjectActivityModel>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<ProjectActivityModel?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<ProjectActivityModel> AddAsync(CreateProjectActivityRequest model, CancellationToken cancellationToken = default);
    Task<ProjectActivityModel?> UpdateAsync(long id, UpdateProjectActivityRequest model, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(long id, CancellationToken cancellationToken = default);
}
