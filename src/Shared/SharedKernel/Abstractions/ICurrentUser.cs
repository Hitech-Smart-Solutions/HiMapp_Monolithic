namespace Himapp.SharedKernel.Abstractions;

public interface ICurrentUser
{
    int? UserId { get; }
    IReadOnlyCollection<string> Permissions { get; }
}

public sealed class AnonymousCurrentUser : ICurrentUser
{
    public int? UserId => null;
    public IReadOnlyCollection<string> Permissions => [];
}
