namespace Himapp.Integrations.D365.Staging;

public sealed record D365Activity(string ActivityCode, string Description, string Uom, DateTimeOffset SyncedAt);
