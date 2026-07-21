using Himapp.SharedKernel.Abstractions;

namespace Himapp.PM.Domain.Equipment;

public sealed class Equipment : BaseEntity
{
    public long ProjectId { get; private set; }
    public string AssetCode { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string Category { get; private set; } = string.Empty;
    public DateOnly? MaintenanceDueOn { get; private set; }
    public string Status { get; private set; } = "Available";

    private Equipment()
    {
    }

    public static Equipment Create(long projectId, string assetCode, string name, string category, DateOnly? maintenanceDueOn)
    {
        return new Equipment
        {
            ProjectId = projectId,
            AssetCode = assetCode,
            Name = name,
            Category = category,
            MaintenanceDueOn = maintenanceDueOn,
            Status = "Available"
        };
    }

    public void Update(string assetCode, string name, string category, DateOnly? maintenanceDueOn)
    {
        AssetCode = assetCode;
        Name = name;
        Category = category;
        MaintenanceDueOn = maintenanceDueOn;
        ModifiedAt = DateTimeOffset.UtcNow;
    }

    public void MarkAsMaintained()
    {
        Status = "Available";
        MaintenanceDueOn = null;
        ModifiedAt = DateTimeOffset.UtcNow;
    }

    public void TransferOut()
    {
        Status = "Transferred";
        ModifiedAt = DateTimeOffset.UtcNow;
    }
}

