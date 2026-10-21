using Himapp.PM.Application.Features.DynamicsEquipmentMasters.Models;
using Himapp.PM.Domain.Entities;

namespace Himapp.PM.Application.Features.DynamicsEquipmentMasters;

internal static class DynamicsEquipmentMasterMapper
{
    public static DynamicsEquipmentMasterDto ToDto(DynamicsEquipmentMaster e) =>
        new()
        {
            Id = e.ID,
            UniqueID = e.UniqueID,
            DataAreaId = e.DataAreaId,
            OBJECTID = e.OBJECTID,
            NAME = e.NAME,
            ACQUISITIONCOST = e.ACQUISITIONCOST,
            ACQUISITIONDATE = e.ACQUISITIONDATE,
            ACTIVEFROM = e.ACTIVEFROM,
            ACTIVETO = e.ACTIVETO,
            CATEGORYLEVEL1 = e.CATEGORYLEVEL1,
            CATEGORYLEVEL2 = e.CATEGORYLEVEL2,
            CATEGORYLEVEL3 = e.CATEGORYLEVEL3,
            CRITICALITY = e.CRITICALITY,
            FIXEDASSETID = e.FIXEDASSETID,
            FIXEDASSETNAME = e.FIXEDASSETNAME,
            FUNCTIONALLOCATION = e.FUNCTIONALLOCATION,
            ISPLBATCHNUMBER = e.ISPLBATCHNUMBER,
            ISPLHIREDDATE = e.ISPLHIREDDATE,
            ISPLISHIREDASSET = e.ISPLISHIREDASSET,
            OBJECTACTIVE = e.OBJECTACTIVE,
            ISPLISSYNC = e.ISPLISSYNC,
            ISPLISUPDATED = e.ISPLISUPDATED,
            ISPLITEMID = e.ISPLITEMID,
            ISPLITEMNAME = e.ISPLITEMNAME,
            ISPLPONUMBER = e.ISPLPONUMBER,
            ISPLVEHICLENUMBER = e.ISPLVEHICLENUMBER,
            ISPLVENDORACCOUNT = e.ISPLVENDORACCOUNT,
            MANUFACTURER = e.MANUFACTURER,
            MODEL = e.MODEL,
            MODELRECID = e.MODELRECID,
            MODELYEAR = e.MODELYEAR,
            OBJECTIDPARENT = e.OBJECTIDPARENT,
            OBJECTIDPARENTNAME = e.OBJECTIDPARENTNAME,
            OBJECTLIFECYCLESTATE = e.OBJECTLIFECYCLESTATE,
            OBJECTTYPE = e.OBJECTTYPE,
            PURCHID = e.PURCHID,
            RESOURCE = e.RESOURCE,
            SERIALID = e.SERIALID,
            SERVICELEVEL = e.SERVICELEVEL,
            STDFUELCONSUMPTION_ISPL = e.STDFUELCONSUMPTION_ISPL,
            VENDACCOUNT = e.VENDACCOUNT,
            VENDNAME = e.VENDNAME,
            WARRANTYDATEFROMVEND = e.WARRANTYDATEFROMVEND,
            WARRANTYDATETOVEND = e.WARRANTYDATETOVEND,
            WARRANTYVEND = e.WARRANTYVEND,
            WRKCTRLDESCRIPTION = e.WRKCTRLDESCRIPTION,
            HimappProjectID = e.HimappProjectID,
            CreatedDate = e.CreatedDate,
            CreatedBy = e.CreatedBy,
            LastModifiedDate = e.LastModifiedDate,
            LastModifiedBy = e.LastModifiedBy
        };

    public static void Apply(DynamicsEquipmentMaster e, DynamicsEquipmentMasterRequest r)
    {
        e.DataAreaId = r.DataAreaId;
        e.OBJECTID = r.OBJECTID;
        e.NAME = r.NAME;
        e.ACQUISITIONCOST = r.ACQUISITIONCOST;
        e.ACQUISITIONDATE = r.ACQUISITIONDATE;
        e.ACTIVEFROM = r.ACTIVEFROM;
        e.ACTIVETO = r.ACTIVETO;
        e.CATEGORYLEVEL1 = r.CATEGORYLEVEL1;
        e.CATEGORYLEVEL2 = r.CATEGORYLEVEL2;
        e.CATEGORYLEVEL3 = r.CATEGORYLEVEL3;
        e.CRITICALITY = r.CRITICALITY;
        e.FIXEDASSETID = r.FIXEDASSETID;
        e.FIXEDASSETNAME = r.FIXEDASSETNAME;
        e.FUNCTIONALLOCATION = r.FUNCTIONALLOCATION;
        e.ISPLBATCHNUMBER = r.ISPLBATCHNUMBER;
        e.ISPLHIREDDATE = r.ISPLHIREDDATE;
        e.ISPLISHIREDASSET = r.ISPLISHIREDASSET;
        e.OBJECTACTIVE = r.OBJECTACTIVE;
        e.ISPLISSYNC = r.ISPLISSYNC;
        e.ISPLISUPDATED = r.ISPLISUPDATED;
        e.ISPLITEMID = r.ISPLITEMID;
        e.ISPLITEMNAME = r.ISPLITEMNAME;
        e.ISPLPONUMBER = r.ISPLPONUMBER;
        e.ISPLVEHICLENUMBER = r.ISPLVEHICLENUMBER;
        e.ISPLVENDORACCOUNT = r.ISPLVENDORACCOUNT;
        e.MANUFACTURER = r.MANUFACTURER;
        e.MODEL = r.MODEL;
        e.MODELRECID = r.MODELRECID;
        e.MODELYEAR = r.MODELYEAR;
        e.OBJECTIDPARENT = r.OBJECTIDPARENT;
        e.OBJECTIDPARENTNAME = r.OBJECTIDPARENTNAME;
        e.OBJECTLIFECYCLESTATE = r.OBJECTLIFECYCLESTATE;
        e.OBJECTTYPE = r.OBJECTTYPE;
        e.PURCHID = r.PURCHID;
        e.RESOURCE = r.RESOURCE;
        e.SERIALID = r.SERIALID;
        e.SERVICELEVEL = r.SERVICELEVEL;
        e.STDFUELCONSUMPTION_ISPL = r.STDFUELCONSUMPTION_ISPL;
        e.VENDACCOUNT = r.VENDACCOUNT;
        e.VENDNAME = r.VENDNAME;
        e.WARRANTYDATEFROMVEND = r.WARRANTYDATEFROMVEND;
        e.WARRANTYDATETOVEND = r.WARRANTYDATETOVEND;
        e.WARRANTYVEND = r.WARRANTYVEND;
        e.WRKCTRLDESCRIPTION = r.WRKCTRLDESCRIPTION;
        e.HimappProjectID = r.HimappProjectID;
    }
}
