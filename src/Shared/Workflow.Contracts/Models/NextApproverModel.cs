using System;
using System.Collections.Generic;
using System.Text;

namespace Himapp.Workflow.Contracts.Models;

public sealed record NextApproverModel(
    int ApproverId,
    string ApproverName
);
