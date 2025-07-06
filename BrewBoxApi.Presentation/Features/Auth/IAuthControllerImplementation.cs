using System.Security.Claims;
using BrewBoxApi.Presentation.Features.Auth.LoginCommand;
using BrewBoxApi.Presentation.Features.Auth.Models;

namespace BrewBoxApi.Presentation.Features.Auth;

public interface IAuthControllerImplementation
{
    ValueTask<AuthView> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
    Task<AuthView> ExternalLoginAsync(ClaimsPrincipal claims, CancellationToken cancellationToken = default);
}