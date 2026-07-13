namespace Himapp.Integrations.D365.Staging;

public sealed record D365Item(string ItemNumber, string Name, string Uom, DateTimeOffset SyncedAt);
