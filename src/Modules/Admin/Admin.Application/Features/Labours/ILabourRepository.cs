using LabourEntity = Himapp.Admin.Domain.Labour.Labour;

namespace Himapp.Admin.Application.Features.Labours;

public interface ILabourRepository
{
    Task<IReadOnlyCollection<LabourEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<LabourEntity?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task AddAsync(LabourEntity labour, CancellationToken cancellationToken = default);
    Task UpdateAsync(LabourEntity labour, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(long id, CancellationToken cancellationToken = default);
}
