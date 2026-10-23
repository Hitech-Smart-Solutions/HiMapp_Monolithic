using Himapp.PM.Application.Features.DynamicsEquipmentCategories.Models;
using Himapp.PM.Domain.Entities;

namespace Himapp.PM.Application.Features.DynamicsEquipmentCategories;

internal static class DynamicsEquipmentCategoryMapper
{
    public static DynamicsEquipmentCategoryDto ToDto(DynamicsEquipmentCategory e) =>
        new()
        {
            Id = e.ID,
            UniqueID = e.UniqueID,
            DataAreaId = e.DataAreaId,
            OBJECTTYPEID = e.OBJECTTYPEID,
            NAME = e.NAME,
            OBJECTLIFECYCLEMODELID = e.OBJECTLIFECYCLEMODELID,
            SHORTNAME_ISPL = e.SHORTNAME_ISPL,
            ISFUELCONSUMPTIONREQUIRED_ISPL = e.ISFUELCONSUMPTIONREQUIRED_ISPL,
            ISLOGBOOKREQUIRED_ISPL = e.ISLOGBOOKREQUIRED_ISPL,
            ISPLISSYNC = e.ISPLISSYNC,
            ISPLISUPDATED = e.ISPLISUPDATED,
            IsActive = e.IsActive,
            CreatedDate = e.CreatedDate,
            CreatedBy = e.CreatedBy,
            LastModifiedDate = e.LastModifiedDate,
            LastModifiedBy = e.LastModifiedBy
        };

    public static void Apply(DynamicsEquipmentCategory e, DynamicsEquipmentCategoryRequest r)
    {
        e.DataAreaId = r.DataAreaId;
        e.OBJECTTYPEID = r.OBJECTTYPEID;
        e.NAME = r.NAME;
        e.OBJECTLIFECYCLEMODELID = r.OBJECTLIFECYCLEMODELID;
        e.SHORTNAME_ISPL = r.SHORTNAME_ISPL;
        e.ISFUELCONSUMPTIONREQUIRED_ISPL = r.ISFUELCONSUMPTIONREQUIRED_ISPL;
        e.ISLOGBOOKREQUIRED_ISPL = r.ISLOGBOOKREQUIRED_ISPL;
        e.ISPLISSYNC = r.ISPLISSYNC;
        e.ISPLISUPDATED = r.ISPLISUPDATED;
        if (r.IsActive.HasValue) e.IsActive = r.IsActive.Value;
    }
}
