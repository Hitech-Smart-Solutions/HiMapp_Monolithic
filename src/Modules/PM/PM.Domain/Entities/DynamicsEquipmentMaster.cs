

using Himapp.PM.Domain.Constants.Enums;
using Himapp.SharedKernel.Abstractions;

namespace Himapp.PM.Domain.Entities;
    /// <summary>
    /// Represents a Dynamics Asset Master Integration entity within the system.
    /// </summary>
    /// <remarks>
    /// Business Meaning:
    /// - Stores core Dynamics Asset Master Integration information used by business workflows
    ///
    /// Lifecycle:
    /// - Created and maintained through application services and data operations
    ///
    /// Change Impact:
    /// - New properties require:
    ///   - Database migration
    ///   - DTO mapping updates
    ///   - Validation and integration review
    /// </remarks>
    public sealed class DynamicsEquipmentMaster : BaseEntity
    {
        public string DataAreaId { get; set; }
        public string OBJECTID { get; set; } = null!;
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
        public ISPLYesNo ISPLISHIREDASSET { get; set; }
        public ISPLYesNo OBJECTACTIVE { get; set; }
        public ISPLISSync ISPLISSYNC { get; set; }
        public ISPLISUpdated ISPLISUPDATED { get; set; }
        public string? ISPLITEMID { get; set; }
        public string? ISPLITEMNAME { get; set; }
        public string? ISPLPONUMBER { get; set; }
        public string? ISPLVEHICLENUMBER { get; set; }
        public string? ISPLVENDORACCOUNT { get; set; }
        public string? MANUFACTURER { get; set; }
        public string? MODEL { get; set; }
        public long? MODELRECID { get; set; }
        public string? MODELYEAR { get; set; }
        public string NAME { get; set; }
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

        private DynamicsEquipmentMaster()
        {
        }

        public static DynamicsEquipmentMaster Register(string dataAreaId, string objectId, decimal? acquisitionCost, DateTime? acquisitionDate, DateTime? activeFrom, DateTime? activeTo, string? categoryLevel1, string? categoryLevel2, string? categoryLevel3, int? criticality, string? fixedAssetId, string? fixedAssetName, string? functionalAllocation, string? isplBatchNumber, DateTime? isplHiredDate, ISPLYesNo isplIsHiredAsset, ISPLYesNo objectActive, ISPLISSync isplIsSync, ISPLISUpdated isplIsUpdated, string? isplItemId, string? isplItemName, string? isplPoNumber, string? isplVehicleNumber, string? isplVendorAccount, string? manufacturer, string? model, long? modelRecId, string? modelYear, string name, string? objectIdParent, string? objectIdParentName, string? objectLifecycleState, string? objectType, string? purchId, string? resource, string? serialId, int? serviceLevel, float? stdFuelConsumptionIsPl, string? vendAccount, string? vendName, DateTime? warrantyDateFromVend, DateTime? warrantyDateToVend, string? warrantyVend)
        {
            return new DynamicsEquipmentMaster
            {
                UniqueID = Guid.NewGuid(),
                DataAreaId = dataAreaId,
                OBJECTID = objectId,
                ACQUISITIONCOST = acquisitionCost,
                ACQUISITIONDATE = acquisitionDate,
                ACTIVEFROM = activeFrom,
                ACTIVETO = activeTo,
                CATEGORYLEVEL1 = categoryLevel1,
                CATEGORYLEVEL2 = categoryLevel2,
                CATEGORYLEVEL3 = categoryLevel3,
                CRITICALITY = criticality,
                FIXEDASSETID = fixedAssetId,
                FIXEDASSETNAME = fixedAssetName,
                FUNCTIONALLOCATION = functionalAllocation,
                ISPLBATCHNUMBER = isplBatchNumber,
                ISPLHIREDDATE = isplHiredDate,
                ISPLISHIREDASSET = isplIsHiredAsset,
                OBJECTACTIVE = objectActive,
                ISPLISSYNC = isplIsSync,
                ISPLISUPDATED = isplIsUpdated,
                ISPLITEMID = isplItemId,
                ISPLITEMNAME = isplItemName,
                ISPLPONUMBER = isplPoNumber,
                ISPLVEHICLENUMBER = isplVehicleNumber,
                ISPLVENDORACCOUNT = isplVendorAccount,
                MANUFACTURER = manufacturer,
                MODEL = model,
                MODELRECID = modelRecId,
                MODELYEAR = modelYear,
                NAME = name,
                OBJECTIDPARENT = objectIdParent,
                OBJECTIDPARENTNAME = objectIdParentName,
                OBJECTLIFECYCLESTATE = objectLifecycleState,
                OBJECTTYPE = objectType,
                PURCHID = purchId,
                RESOURCE = resource,
                SERIALID = serialId,
                SERVICELEVEL = serviceLevel,
                STDFUELCONSUMPTION_ISPL = stdFuelConsumptionIsPl,
                VENDACCOUNT = vendAccount,
                VENDNAME = vendName,
                WARRANTYDATEFROMVEND = warrantyDateFromVend,
                WARRANTYDATETOVEND = warrantyDateToVend,
                WARRANTYVEND = warrantyVend,
                CreatedAt = DateTimeOffset.UtcNow,
                ModifiedAt = DateTimeOffset.UtcNow
            };
        }        
    }