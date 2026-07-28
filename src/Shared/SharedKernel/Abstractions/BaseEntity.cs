namespace Himapp.SharedKernel.Abstractions;

public abstract class BaseEntity
{
    private readonly List<IDomainEvent> _domainEvents = [];

    public int Id { get; protected set; }
    public DateTimeOffset CreatedAt { get; protected set; } = DateTimeOffset.UtcNow;
    public int? CreatedBy { get; protected set; }
    public DateTimeOffset? ModifiedAt { get; protected set; }
    public int? ModifiedBy { get; protected set; }
    public byte[] RowVersion { get; protected set; } = [];

    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected void Raise(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);

    public void ClearDomainEvents() => _domainEvents.Clear();
}
