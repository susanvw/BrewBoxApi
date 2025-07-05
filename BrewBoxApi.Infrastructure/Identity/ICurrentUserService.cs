using System.Security.Claims;

namespace BrewBoxApi.Infrastructure.Identity;

public interface ICurrentUserService
{
    string? UserId { get; }
    string? Email { get; }
    bool IsAuthenticated { get; }
    string? Role { get; }
    string RequestPath { get; }
    IEnumerable<Claim> Claims { get; }
}