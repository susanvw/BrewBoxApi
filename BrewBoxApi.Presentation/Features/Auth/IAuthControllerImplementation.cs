using System.Security.Claims;
using BrewBoxApi.Presentation.Features.Auth.LoginCommand;
using BrewBoxApi.Presentation.Features.Auth.Models;
using BrewBoxApi.Presentation.Features.Auth.RegisterCommand;
using Microsoft.AspNetCore.Identity;

namespace BrewBoxApi.Presentation.Features.Auth;

public interface IAuthControllerImplementation
{
    ValueTask<IdentityResult> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);
    ValueTask<AuthView> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
    Task<AuthView> ExternalLoginAsync(ClaimsPrincipal claims, CancellationToken cancellationToken = default);
}