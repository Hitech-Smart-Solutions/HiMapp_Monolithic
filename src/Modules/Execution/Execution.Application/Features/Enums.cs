using System;
using System.Collections.Generic;
using System.Text;

namespace Himapp.Execution.Application.Features
{
    public enum Actions
    {
        Inserted = 501,
        Updated = 502,
        Deleted = 503,

        // Status
        Activated = 504,
        Inactivated = 505,

        // Approval Workflow
        Viewed = 506
    }
    public enum CategoryType
    {
        Skilled = 1,
        Unskilled = 2,
        Other = 3
    }

    public enum DailyLaborState : short
    {
        Draft = 1,
        Submitted = 2
    }
}
