namespace Himapp.Integrations.D365.Services;

public sealed class NoopD365SyncService : ID365SyncService
{
    public Task SyncItemsAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
    public Task SyncEquipmentAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
    public Task SyncActivitiesAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
}
