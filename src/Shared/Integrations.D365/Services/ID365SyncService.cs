namespace Himapp.Integrations.D365.Services;

public interface ID365SyncService
{
    Task SyncItemsAsync(CancellationToken cancellationToken = default);
    Task SyncEquipmentAsync(CancellationToken cancellationToken = default);
    Task SyncActivitiesAsync(CancellationToken cancellationToken = default);
}
