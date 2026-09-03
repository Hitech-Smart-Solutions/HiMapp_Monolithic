using System;
using System.Collections.Generic;
using System.Text;

namespace Himapp.Execution.Application.Features.SiteDailyProgress.Models
{
    public class ActivityWiseQuantityBySectionModel
    {
        public int SectionID { get; set; }
        public string? SectionName { get; set; }
        public int ActivityID { get; set; }
        public string? ActivityName { get; set; }
        public decimal TargetQuantity { get; set; }
        public decimal ActualQuantity { get; set; }
        public int UOMID { get; set; }
        public string? UOMName { get; set; }
        public string? UOMShortName { get; set; }
        public decimal RevenueRate { get; set; }
    }
}
