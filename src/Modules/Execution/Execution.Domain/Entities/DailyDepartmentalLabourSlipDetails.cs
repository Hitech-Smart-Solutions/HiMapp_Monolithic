using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Himapp.Execution.Domain.Entities
{
    // EF entity for schema table "DailyDepartmentalLabourSlipDetails" Page Name : Daily Department Labour Slip (DDLS)
    public class DailyDepartmentalLabourSlipDetails
    {
        public DailyDepartmentalLabourSlipDetails()
        {

        }

        public int ID { get; set; }
        public Guid UniqueID { get; set; }

        public int? DDLSlipID { get; set; }
        public int? LabourCategoryTypeID { get; set; }
        public int? NumOfLabour { get; set; }

        public DateTime FromTime { get; set; }
        public DateTime TOTime { get; set; }
        public decimal? LunchHour { get; set; }
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

        public bool? IsLumSumWork { get; set; } = false;

        [ForeignKey("DDLSlipID")]
        public virtual DailyDepartmentalLabourSlip? DailyDepartmentalLabourSlip { get; set; }
    }
}
