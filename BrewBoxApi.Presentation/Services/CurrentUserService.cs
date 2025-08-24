using System.Security.Claims;
using System.Threading.Tasks;
using BrewBoxApi.Application.Common.Identity;
using BrewBoxApi.Domain.Aggregates.Identity;
using Microsoft.AspNetCore.Identity;

namespace BrewBoxApi.Presentation.Services;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager) : ICurrentUserService
{
    public string? UserId => httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    public string? Email => httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Email)?.Value;

    public bool IsAuthenticated => httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

    public string RequestPath => httpContextAccessor.HttpContext?.Request?.Path.Value ?? string.Empty;

    public IEnumerable<Claim> Claims => httpContextAccessor.HttpContext?.User?.Claims ?? [];

    public async Task<bool> IsInRoleAsync(RoleType role)
    {
        if (UserId == null)
            return false;

        var user = await userManager.FindByIdAsync(UserId);

        if (user == null)
            return false;

        return await userManager.IsInRoleAsync(user, role.ToString());
    }
}