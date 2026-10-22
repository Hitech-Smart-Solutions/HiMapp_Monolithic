namespace Himapp.PM.Domain.Constants;

public static class DynamicsSyncEntity
{
    public const string AssetType = "AssetType";
    public const string AssetMaster = "AssetMaster";
}

public static class DynamicsSyncMode
{
    public const string Add = "Add";
    public const string Update = "Update";
    public const string Reset = "Reset";
}

public static class DynamicsSyncTrigger
{
    public const string Manual = "Manual";
    public const string BackgroundJob = "BackgroundJob";
}

public static class DynamicsSyncStatus
{
    public const string Queued = "Queued";
    public const string Running = "Running";
    public const string Success = "Success";
    public const string Failed = "Failed";
    public const string NothingToSync = "NothingToSync";
}
