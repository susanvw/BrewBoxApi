using System.Security.Claims;
using BrewBoxApi.Domain.Aggregates.Identity;

namespace BrewBoxApi.Application.Common.Identity;

public interface ICurrentUserService
{
    string? UserId { get; }
    string? Email { get; }
    bool IsAuthenticated { get; }
    string RequestPath { get; }
    IEnumerable<Claim> Claims { get; }

    Task<bool> IsInRoleAsync(RoleType role);
}