using System.Security.Claims;
using BrewBoxApi.Presentation.Features.Auth.LoginCommand;
using BrewBoxApi.Presentation.Features.Auth.Models;
using BrewBoxApi.Presentation.Features.SeedWork;

namespace BrewBoxApi.Presentation.Features.Auth;

public interface IAuthControllerImplementation
{
    ValueTask<BaseResponse<AuthView>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
    ValueTask<BaseResponse<AuthView>> ExternalLoginAsync(ClaimsPrincipal claims, CancellationToken cancellationToken = default);
}