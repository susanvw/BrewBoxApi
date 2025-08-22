using System.Security.Claims;
using BrewBoxApi.Application.Common.Identity;

namespace BrewBoxApi.Presentation.Services;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    public string? UserId => httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    public string? Email => httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Email)?.Value;

    public bool IsAuthenticated => httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

    public string? Role => httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Role)?.Value;

    public string RequestPath => httpContextAccessor.HttpContext?.Request?.Path.Value ?? string.Empty;

    public IEnumerable<Claim> Claims => httpContextAccessor.HttpContext?.User?.Claims ?? [];
}