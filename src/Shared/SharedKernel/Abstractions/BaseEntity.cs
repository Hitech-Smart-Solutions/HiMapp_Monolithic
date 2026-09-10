using System.ComponentModel.DataAnnotations.Schema;

namespace Himapp.SharedKernel.Abstractions;

public abstract class BaseEntity
{
    private readonly List<IDomainEvent> _domainEvents = new();
    public Guid UniqueID { get; set; } = Guid.NewGuid();
    public int ID { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public int CreatedBy { get; set; }
    public DateTime LastModifiedDate { get; set; }
    public int LastModifiedBy { get; set; }


    [NotMapped]
    public int Id
    {
        get => ID;
        set => ID = value;
    }

    [NotMapped]
    public DateTimeOffset CreatedAt
    {
        get => new DateTimeOffset(CreatedDate);
        set => CreatedDate = value.UtcDateTime;
    }

    [NotMapped]
    public DateTimeOffset? ModifiedAt
    {
        get => LastModifiedDate == default ? null : new DateTimeOffset(LastModifiedDate);
        set => LastModifiedDate = value?.UtcDateTime ?? default;
    }

    [NotMapped]
    public int? ModifiedBy
    {
        get => LastModifiedBy == 0 ? null : LastModifiedBy;
        set => LastModifiedBy = value ?? 0;
    }

    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected void Raise(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);

    public void ClearDomainEvents() => _domainEvents.Clear();
}
