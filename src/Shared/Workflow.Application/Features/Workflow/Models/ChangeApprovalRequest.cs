using System;
using System.Collections.Generic;
using System.Text;

namespace Himapp.Workflow.Application.Features.Workflow.Models;

public sealed record ChangeApprovalRequest(
    int Id,
    int ProjectId,
    int EntityId,
    short StatusId,
    string? Remarks,
    int NextApproverId,
    int Priority
);
