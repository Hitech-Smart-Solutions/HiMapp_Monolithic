namespace Himapp.PM.Domain.Constants;

/// <summary>Overall state of the equipment setup wizard.</summary>
public static class EquipmentSetupStatus
{
    public const string Draft = "Draft";
    public const string Completed = "Completed";
}

/// <summary>State of one wizard step as stored. "Locked" is never stored; it is worked out from the other steps.</summary>
public static class EquipmentSetupStepStatus
{
    public const string InProgress = "InProgress";
    public const string Complete = "Complete";
    public const string Locked = "Locked";
}

/// <summary>Where a PM point / specification of an equipment came from.</summary>
public static class EquipmentSetupSource
{
    /// <summary>Suggested by the equipment category.</summary>
    public const string Category = "Category";
    /// <summary>Added by hand from the master ("Add PM point" / "Add specification").</summary>
    public const string Master = "Master";
}

public static class EquipmentSetupStepNo
{
    public const int Details = 1;
    public const int Information = 2;
    public const int MeterFuel = 3;
    public const int Documents = 4;
    public const int PreventiveMaintenance = 5;
    public const int Specifications = 6;
    public const int Review = 7;
    public const int Complete = 8;
    public const int Total = 8;
}
