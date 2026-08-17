using System;
using System.Collections.Generic;
using System.Text;

namespace Himapp.Execution.Domain.Entities
{
    // EF entity for schema table "DailyProgressHindrances"
    // Page Name : Site DPR
    public class SiteDailyProgressHindrance
    {
        public int ID { get; set; }

        public Guid UniqueID { get; set; }

        public int DailyProgressID { get; set; }

        public string? Hindrance { get; set; } = string.Empty;
        
        public string? AudioUrl { get; set; }

        public bool IsActive { get; set; }

        public int? CreatedBy { get; set; }
        public DateTimeOffset CreatedDate { get; set; }

        public int? LastModifiedBy { get; set; }
        public DateTimeOffset LastModifiedDate { get; set; }

        public virtual SiteDailyProgress? DailyProgress { get; set; }
    }
}
