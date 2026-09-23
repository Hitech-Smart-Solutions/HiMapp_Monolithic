using System;
using System.Collections.Generic;
using System.Text;

namespace Himapp.Execution.Application.Features.DailyProgress.Models
{
    public class CheckTransactionLockModel
    {
        public int ProgramID { get; set; }
        public int ProjectID { get; set; }
        public int SectionID { get; set; }
        public DateTime ReportDate { get; set; }
    }
}
