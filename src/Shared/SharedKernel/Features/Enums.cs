using System;
using System.Collections.Generic;
using System.Text;

namespace Himapp.SharedKernel.Features
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

    public enum ApprovalStatus
    {
        Draft = 1,
        Pending = 2,
        Approved = 3,
        Rejected = 4
    }

    public enum EquipmentChecklistPointType
    {
        Daily = 1,
        ECR = 2
    }

    public enum EquipmentBreakdownReasonType
    {
        Hydraulic = 1,
        Electrical = 2,
        Mechanical = 3
    }

    public enum SpecificationType
    {
        Numeric = 1,
        AlphaNumeric = 2,
        DateAndTime = 3
    }
}
