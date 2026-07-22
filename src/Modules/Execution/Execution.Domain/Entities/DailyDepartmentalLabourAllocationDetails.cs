using System;
using System.Collections.Generic;
using System.Text;

namespace Himapp.Execution.Domain.Entities
{
    public class DailyDepartmentalLabourAllocationDetails
    {
        public int ID { get; set; }
        public Guid UniqueID { get; set; }

        public int? DDLSlipID { get; set; }
        public int? DDLSlipDetailID { get; set; }

        public decimal? WorkingHours { get; set; }
        public int? WorkLocationID { get; set; }
        public int? ActivityCategoryID { get; set; }
        public string? ActivityDetails { get; set; }
        public int? UOMID { get; set; }
        public decimal? Quantity { get; set; }
        public int? DebitPartyID { get; set; }
        public string? Remarks { get; set; }

        public short? StateID { get; set; }
        public bool IsActive { get; set; } = true;
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int LastModifiedBy { get; set; }
        public DateTime LastModifiedDate { get; set; }

        public virtual DailyDepartmentalLabourSlipDetails? DailyDepartmentalLabourSlipDetails { get; set; }
    }
}
