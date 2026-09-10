using System;
using System.Collections.Generic;
using System.Text;

namespace Himapp.Workflow.Contracts.References;

public interface IRequiresApproval
{
    int Id { get; }
    int ProjectId { get; }
    int ProgramId { get; }
    int EntityId { get; }
    short StatusId { get; }
    string? Remarks { get; }
    int Priority { get; }
}
