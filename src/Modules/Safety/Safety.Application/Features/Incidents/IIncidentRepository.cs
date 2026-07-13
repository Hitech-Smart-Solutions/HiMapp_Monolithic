namespace Himapp.Safety.Application.Features.Incidents;

internal interface IIncidentRepository
{
    Task<IReadOnlyCollection<IncidentDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IncidentDto?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<IncidentDto> AddAsync(IncidentDto incident, CancellationToken cancellationToken = default);
    Task<IncidentDto?> UpdateAsync(IncidentDto incident, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(long id, CancellationToken cancellationToken = default);
}
