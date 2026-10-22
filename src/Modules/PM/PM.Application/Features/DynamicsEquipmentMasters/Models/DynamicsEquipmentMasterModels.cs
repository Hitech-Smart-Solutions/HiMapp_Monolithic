using Himapp.PM.Domain.Constants.Enums;

namespace Himapp.PM.Application.Features.DynamicsEquipmentMasters.Models;

/// <summary>Request body for create and update.</summary>
public sealed class DynamicsEquipmentMasterRequest
{
    public string DataAreaId { get; set; } = string.Empty;
    public string OBJECTID { get; set; } = string.Empty;
    public string NAME { get; set; } = string.Empty;
    public decimal? ACQUISITIONCOST { get; set; }
    public DateTime? ACQUISITIONDATE { get; set; }
    public DateTime? ACTIVEFROM { get; set; }
    public DateTime? ACTIVETO { get; set; }
    public string? CATEGORYLEVEL1 { get; set; }
    public string? CATEGORYLEVEL2 { get; set; }
    public string? CATEGORYLEVEL3 { get; set; }
    public int? CRITICALITY { get; set; }
    public string? FIXEDASSETID { get; set; }
    public string? FIXEDASSETNAME { get; set; }
    public string? FUNCTIONALLOCATION { get; set; }
    public string? ISPLBATCHNUMBER { get; set; }
    public DateTime? ISPLHIREDDATE { get; set; }
    public ISPLYesNo ISPLISHIREDASSET { get; set; } = ISPLYesNo.No;
    public ISPLYesNo OBJECTACTIVE { get; set; } = ISPLYesNo.No;
    public ISPLISSync ISPLISSYNC { get; set; } = ISPLISSync.No;
    public ISPLISUpdated ISPLISUPDATED { get; set; } = ISPLISUpdated.No;
    public string? ISPLITEMID { get; set; }
    public string? ISPLITEMNAME { get; set; }
    public string? ISPLPONUMBER { get; set; }
    public string? ISPLVEHICLENUMBER { get; set; }
    public string? ISPLVENDORACCOUNT { get; set; }
    public string? MANUFACTURER { get; set; }
    public string? MODEL { get; set; }
    public long? MODELRECID { get; set; }
    public string? MODELYEAR { get; set; }
    public string? OBJECTIDPARENT { get; set; }
    public string? OBJECTIDPARENTNAME { get; set; }
    public string? OBJECTLIFECYCLESTATE { get; set; }
    public string? OBJECTTYPE { get; set; }
    public string? PURCHID { get; set; }
    public string? RESOURCE { get; set; }
    public string? SERIALID { get; set; }
    public int? SERVICELEVEL { get; set; }
    public float? STDFUELCONSUMPTION_ISPL { get; set; }
    public string? VENDACCOUNT { get; set; }
    public string? VENDNAME { get; set; }
    public DateTime? WARRANTYDATEFROMVEND { get; set; }
    public DateTime? WARRANTYDATETOVEND { get; set; }
    public string? WARRANTYVEND { get; set; }
    public string? WRKCTRLDESCRIPTION { get; set; }
    public int? HimappProjectID { get; set; }
}

public sealed class DynamicsEquipmentMasterDto
{
    public int Id { get; set; }
    public Guid UniqueID { get; set; }
    public string DataAreaId { get; set; } = string.Empty;
    public string OBJECTID { get; set; } = string.Empty;
    public string NAME { get; set; } = string.Empty;
    public decimal? ACQUISITIONCOST { get; set; }
    public DateTime? ACQUISITIONDATE { get; set; }
    public DateTime? ACTIVEFROM { get; set; }
    public DateTime? ACTIVETO { get; set; }
    public string? CATEGORYLEVEL1 { get; set; }
    public string? CATEGORYLEVEL2 { get; set; }
    public string? CATEGORYLEVEL3 { get; set; }
    public int? CRITICALITY { get; set; }
    public string? FIXEDASSETID { get; set; }
    public string? FIXEDASSETNAME { get; set; }
    public string? FUNCTIONALLOCATION { get; set; }
    public string? ISPLBATCHNUMBER { get; set; }
    public DateTime? ISPLHIREDDATE { get; set; }
    public ISPLYesNo ISPLISHIREDASSET { get; set; } = ISPLYesNo.No;
    public ISPLYesNo OBJECTACTIVE { get; set; } = ISPLYesNo.No;
    public ISPLISSync ISPLISSYNC { get; set; } = ISPLISSync.No;
    public ISPLISUpdated ISPLISUPDATED { get; set; } = ISPLISUpdated.No;
    public string? ISPLITEMID { get; set; }
    public string? ISPLITEMNAME { get; set; }
    public string? ISPLPONUMBER { get; set; }
    public string? ISPLVEHICLENUMBER { get; set; }
    public string? ISPLVENDORACCOUNT { get; set; }
    public string? MANUFACTURER { get; set; }
    public string? MODEL { get; set; }
    public long? MODELRECID { get; set; }
    public string? MODELYEAR { get; set; }
    public string? OBJECTIDPARENT { get; set; }
    public string? OBJECTIDPARENTNAME { get; set; }
    public string? OBJECTLIFECYCLESTATE { get; set; }
    public string? OBJECTTYPE { get; set; }
    public string? PURCHID { get; set; }
    public string? RESOURCE { get; set; }
    public string? SERIALID { get; set; }
    public int? SERVICELEVEL { get; set; }
    public float? STDFUELCONSUMPTION_ISPL { get; set; }
    public string? VENDACCOUNT { get; set; }
    public string? VENDNAME { get; set; }
    public DateTime? WARRANTYDATEFROMVEND { get; set; }
    public DateTime? WARRANTYDATETOVEND { get; set; }
    public string? WARRANTYVEND { get; set; }
    public string? WRKCTRLDESCRIPTION { get; set; }
    public int? HimappProjectID { get; set; }
    public DateTime CreatedDate { get; set; }
    public int CreatedBy { get; set; }
    public DateTime LastModifiedDate { get; set; }
    public int LastModifiedBy { get; set; }
}
