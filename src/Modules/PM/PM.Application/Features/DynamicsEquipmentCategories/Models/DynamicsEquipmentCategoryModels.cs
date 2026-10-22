using Himapp.PM.Domain.Constants.Enums;

namespace Himapp.PM.Application.Features.DynamicsEquipmentCategories.Models;

/// <summary>Request body for create and update.</summary>
public sealed class DynamicsEquipmentCategoryRequest
{
    public string DataAreaId { get; set; } = string.Empty;
    public string OBJECTTYPEID { get; set; } = string.Empty;
    public string NAME { get; set; } = string.Empty;
    public string OBJECTLIFECYCLEMODELID { get; set; } = string.Empty;
    public string SHORTNAME_ISPL { get; set; } = string.Empty;
    public ISPLYesNo ISFUELCONSUMPTIONREQUIRED_ISPL { get; set; } = ISPLYesNo.No;
    public ISPLYesNo ISLOGBOOKREQUIRED_ISPL { get; set; } = ISPLYesNo.No;
    public ISPLISSync ISPLISSYNC { get; set; } = ISPLISSync.No;
    public ISPLISUpdated ISPLISUPDATED { get; set; } = ISPLISUpdated.No;
}

public sealed class DynamicsEquipmentCategoryDto
{
    public int Id { get; set; }
    public Guid UniqueID { get; set; }
    public string DataAreaId { get; set; } = string.Empty;
    public string OBJECTTYPEID { get; set; } = string.Empty;
    public string NAME { get; set; } = string.Empty;
    public string OBJECTLIFECYCLEMODELID { get; set; } = string.Empty;
    public string SHORTNAME_ISPL { get; set; } = string.Empty;
    public ISPLYesNo ISFUELCONSUMPTIONREQUIRED_ISPL { get; set; } = ISPLYesNo.No;
    public ISPLYesNo ISLOGBOOKREQUIRED_ISPL { get; set; } = ISPLYesNo.No;
    public ISPLISSync ISPLISSYNC { get; set; } = ISPLISSync.No;
    public ISPLISUpdated ISPLISUPDATED { get; set; } = ISPLISUpdated.No;
    // public bool IsActive { get; set; }
    public DateTime? CreatedDate { get; set; }
    public int CreatedBy { get; set; }
    public DateTime? LastModifiedDate { get; set; }
    public int LastModifiedBy { get; set; }
}
