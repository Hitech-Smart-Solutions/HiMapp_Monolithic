namespace Himapp.SharedKernel.Abstractions;

using System.Security.Claims;
using Microsoft.AspNetCore.Http;

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

/// <summary>
/// Resolves the current user from the claims populated by JWT bearer authentication.
/// </summary>
public sealed class JwtCurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    // Support the standard JWT subject claim and the user-id claim names used by
    // common identity providers. NameIdentifier also covers JWT inbound-claim mapping.
    private static readonly string[] UserIdClaimTypes =
    [
        ClaimTypes.NameIdentifier,
        "sub",
        "user_id",
        "userId",
        "userid",
        "UserId",
        "id"
    ];

    public int? UserId
    {
        get
        {
            var user = httpContextAccessor.HttpContext?.User;
            if (user?.Identity?.IsAuthenticated != true)
            {
                return null;
            }

            foreach (var claimType in UserIdClaimTypes)
            {
                var value = user.FindFirst(claimType)?.Value;
                if (int.TryParse(value, out var userId) && userId > 0)
                {
                    return userId;
                }
            }

            return null;
        }
    }

    public IReadOnlyCollection<string> Permissions =>
        httpContextAccessor.HttpContext?.User
            .FindAll("permission")
            .Select(claim => claim.Value)
            .ToArray()
        ?? [];
}
