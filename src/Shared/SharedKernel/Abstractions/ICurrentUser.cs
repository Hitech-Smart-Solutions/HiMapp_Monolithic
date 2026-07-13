namespace Himapp.SharedKernel.Abstractions;

public interface ICurrentUser
{
    long? UserId { get; }
    IReadOnlyCollection<string> Permissions { get; }
}

public sealed class AnonymousCurrentUser : ICurrentUser
{
    public long? UserId => null;
    public IReadOnlyCollection<string> Permissions => [];
}
