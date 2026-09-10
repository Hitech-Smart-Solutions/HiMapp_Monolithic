using Himapp.SharedKernel.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Himapp.Execution.Domain.Entities
{
    // EF entity for schema table "DailyProgressHindrances"
    // Page Name : Site DPR
    public class SiteDailyProgressHindrance : BaseEntity
    {

        public int DailyProgressID { get; set; }

        public string? Hindrance { get; set; } = string.Empty;
        
        public string? AudioUrl { get; set; }

        public bool IsActive { get; set; }

        public virtual SiteDailyProgress? DailyProgress { get; set; }
    }
}
