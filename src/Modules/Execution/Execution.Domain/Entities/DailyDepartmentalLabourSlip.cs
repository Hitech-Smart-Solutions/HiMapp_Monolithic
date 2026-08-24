using System;
using System.Collections.Generic;
using System.Text;

namespace Himapp.Execution.Domain.Entities
{
    // EF entity for schema table "DailyDepartmentalLabourSlip" Page Name : Daily Department Labour Slip (DDLS)
    public class DailyDepartmentalLabourSlip
    {
        public DailyDepartmentalLabourSlip()
        {
            DailyDepartmentalLabourSlipDetails = new HashSet<DailyDepartmentalLabourSlipDetails>();
        }

        public int ID { get; set; }
        public Guid UniqueID { get; set; }

        public int? ProjectID { get; set; }

        public string? DDLSlipCode { get; set; }
        public DateTime? SlipDate { get; set; }
        public string? IssueNumber { get; set; }

        public int? PartyID { get; set; }
        public bool? IsNewParty { get; set; }
        public string? NewParty { get; set; }

        public string? Remarks { get; set; }
        public short? StateID { get; set; }
        public bool IsActive { get; set; } = true;

        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int LastModifiedBy { get; set; }
        public DateTime LastModifiedDate { get; set; }

        public string? DocumentName { get; set; }
        public string? DocumentContentType { get; set; }
        public string? DocumentPath { get; set; }

        public int? IsDisapproved { get; set; }
        public int? TotalWrkMins { get; set; }
        public int? DPRSlipIssueID { get; set; }
        public int? TotalDPRManpower { get; set; }
        public int? Skilled { get; set; }
        public int? UnSkilled { get; set; }
        public int? Mat { get; set; }

        public virtual ICollection<DailyDepartmentalLabourSlipDetails> DailyDepartmentalLabourSlipDetails { get; set; }
    }
}
