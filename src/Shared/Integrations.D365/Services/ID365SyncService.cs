namespace Himapp.Integrations.D365.Services;

public interface ID365SyncService
{
    Task SyncItemsAsync(CancellationToken cancellationToken = default);
    Task SyncEquipmentAsync(CancellationToken cancellationToken = default);
    Task SyncActivitiesAsync(CancellationToken cancellationToken = default);
    Task<List<T>> FetchFromDynamics<T>(string entityName, string filter);
    Task<string> GetDynamicsToken();
    Task<bool> PatchToDynamics(string resourcePath, string token, object payload);
    Task<bool> PostToDynamics(string resourcePath, string token, object payload);
    DateTime? NormalizeDynamicsDate(DateTime? date = null);
}
