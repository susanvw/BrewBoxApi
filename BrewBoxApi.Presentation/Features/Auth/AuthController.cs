using System.Security.Claims;
using BrewBoxApi.Presentation.Features.Auth.LoginCommand;
using Microsoft.AspNetCore.Mvc;

namespace BrewBoxApi.Presentation.Features.Auth;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthControllerImplementation implementation) : ControllerBase
{
    [HttpPost("login")]
    public async Task<ActionResult> LoginAsync([FromBody] LoginRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await implementation.LoginAsync(request, cancellationToken);
        if (!result.Success)
        {
            return Unauthorized(new { errors = result.Errors });
        }

        return Ok(result);
    }

    [HttpPost("external-login")]
    public async Task<ActionResult> ExternalLoginAsync([FromBody] ExternalLoginRequest request, CancellationToken cancellationToken = default)
    {
        var info = await ValidateExternalTokenAsync(request.Provider, request.Token);
        if (info == null)
        {
            return Unauthorized(new { error = "Invalid external token" });
        }

        var result = await implementation.ExternalLoginAsync(info, cancellationToken);
        
        if (!result.Success)
        {
            return Unauthorized(new { error = result.Errors });
        }

        return Ok(new { result });
    }

    private async Task<ClaimsPrincipal?> ValidateExternalTokenAsync(string provider, string token)
    {
        // Placeholder for Google/Apple token validation
        // In a real implementation, use Google.Apis.Auth or Apple Sign In SDK
        try
        {
            if (provider == "Google")
            {
                // Example: Validate Google ID token (requires Google.Apis.Auth)
                // var payload = await GoogleJsonWebSignature.ValidateAsync(token);
                // return new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Email, payload.Email) }, provider));
                return new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Email, "test@google.com") }, provider));
            }
            else if (provider == "Apple")
            {
                // Example: Validate Apple ID token (requires custom JWT validation)
                return new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Email, "test@apple.com") }, provider));
            }
            return null;
        }
        catch
        {
            return null;
        }
    }
}