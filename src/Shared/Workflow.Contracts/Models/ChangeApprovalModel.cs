using System;
using System.Collections.Generic;
using System.Text;

namespace Himapp.Workflow.Contracts.Models;

public sealed record ChangeApprovalModel(
    int Id,
    int EntityId,
    int StatusId,
    int NextApproverId
);
