namespace Himapp.Integrations.D365.Staging;

public sealed record D365Equipment(string EquipmentNumber, string Description, string Status, DateTimeOffset SyncedAt);
