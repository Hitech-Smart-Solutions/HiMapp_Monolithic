using Himapp.SharedKernel.Abstractions;

namespace Himapp.Safety.Domain.Induction;

public sealed class InductionSession : BaseEntity
{
    public long ProjectId { get; init; }
    public DateOnly SessionDate { get; init; }
    public string TopicSet { get; init; } = string.Empty;
    public List<long> AttendeeLabourIds { get; } = [];
}
