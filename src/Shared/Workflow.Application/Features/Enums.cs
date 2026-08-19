using System;
using System.Collections.Generic;
using System.Text;

namespace Himapp.Workflow.Application.Features
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

    public enum ApprovalWorkflowStatus
    {
        Draft = 1,
        AwaitingApproval = 2,
        Approved = 3,
        DisApproved = 4,
        OnHold = 5,
        ActionNotRequired = 6,
        Revised = 7
    }
}
