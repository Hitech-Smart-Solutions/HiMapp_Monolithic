using System;
using System.Collections.Generic;
using System.Text;

namespace Himapp.Workflow.Application.Features.Workflow.Models;

public sealed record GetNextApproverRequest(
    int ProjectId,
    int ProgramId,
    int Priority
);