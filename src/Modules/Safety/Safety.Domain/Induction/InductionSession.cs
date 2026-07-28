using Himapp.SharedKernel.Abstractions;

namespace Himapp.Safety.Domain.Induction;

public sealed class InductionSession : BaseEntity
{
    public int ProjectId { get; init; }
    public DateOnly SessionDate { get; init; }
    public string TopicSet { get; init; } = string.Empty;
    public List<int> AttendeeLabourIds { get; } = [];
}
