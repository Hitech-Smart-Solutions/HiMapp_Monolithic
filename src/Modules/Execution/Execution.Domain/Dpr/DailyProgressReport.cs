using Himapp.Execution.Domain.Dpr.Events;
using Himapp.SharedKernel.Abstractions;

namespace Himapp.Execution.Domain.Dpr;

public sealed class DailyProgressReport : BaseEntity
{
    public int ProjectId { get; init; }
    public DateOnly WorkDate { get; init; }
    public string Status { get; private set; } = "Draft";

    public void Submit()
    {
        Status = "Submitted";
        Raise(new DprSubmitted(ProjectId, Id, WorkDate));
    }
}
