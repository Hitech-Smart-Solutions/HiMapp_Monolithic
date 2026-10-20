

using Himapp.PM.Domain.Constants.Enums;
using Himapp.SharedKernel.Abstractions;
using System;

namespace Himapp.PM.Domain.Entities;
    /// <summary>
    /// Represents a Dynamics Asset Type entity within the system.
    /// </summary>
    /// <remarks>
    /// Business Meaning:
    /// - Stores core Dynamics Asset Type information used by business workflows
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
    public sealed class DynamicsAssetType : BaseEntity
    {
        public string DataAreaId { get; set; }
        public string OBJECTTYPEID { get; set; }
        public ISPLYesNo ISFUELCONSUMPTIONREQUIRED_ISPL { get; set; }
        public ISPLYesNo ISLOGBOOKREQUIRED_ISPL { get; set; }
        public ISPLISSync ISPLISSYNC { get; set; }
        public ISPLISUpdated ISPLISUPDATED { get; set; }
        public string NAME { get; set; }
        public string OBJECTLIFECYCLEMODELID { get; set; }
        public string SHORTNAME_ISPL { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }

        public DynamicsAssetType()
        {
        }

        public static DynamicsAssetType Register(string dataAreaId, string objectTypeId, ISPLYesNo isFuelConsumptionRequired, ISPLYesNo isLogbookRequired, ISPLISSync isPlIsSync, ISPLISUpdated isPlIsUpdated, string name, string objectLifecycleModelId, string shortNameIsPl)
        {
            return new DynamicsAssetType
            {
                UniqueID = Guid.NewGuid(),
                DataAreaId = dataAreaId,
                OBJECTTYPEID = objectTypeId,
                ISFUELCONSUMPTIONREQUIRED_ISPL = isFuelConsumptionRequired,
                ISLOGBOOKREQUIRED_ISPL = isLogbookRequired,
                ISPLISSYNC = isPlIsSync,
                ISPLISUPDATED = isPlIsUpdated,
                NAME = name,
                OBJECTLIFECYCLEMODELID = objectLifecycleModelId,
                SHORTNAME_ISPL = shortNameIsPl,
                CreatedAt = DateTimeOffset.UtcNow,
                ModifiedAt = DateTimeOffset.UtcNow
            };
        }
    }
