using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BrewBoxApi.Presentation.Features.Account.MfaCommand;
using BrewBoxApi.Presentation.Features.Account.RegisterCommand;
using BrewBoxApi.Presentation.Features.Auth.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Google.Apis.Auth;

namespace BrewBoxApi.Presentation.Features.Account;

internal sealed class AccountControllerImplementation(
UserManager<IdentityUser> userManager,
IConfiguration configuration
) : IAccountControllerImplementation
{

    public async ValueTask<IdentityResult> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var user = new IdentityUser { UserName = request.Email, Email = request.Email };
        var result = await userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            return result;
        }
        await userManager.AddToRoleAsync(user, "Barista");
        return result;
    }

    public async ValueTask<AuthView> VerifyAppleMfaAsync(MfaRequest request, CancellationToken cancellationToken = default)
    {  // Mock Apple Sign-In validation (requires Apple Developer setup in production)
        if (string.IsNullOrEmpty(request.Token) || !request.Token.StartsWith("mock_apple_"))
        {
            return new AuthView { Succeeded = false, Message = "Invalid Apple token" };
        }
        var email = request.Token.Replace("mock_apple_", "");
        var user = await userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return new AuthView { Succeeded = false, Message = "User not found" };
        }
        var token = GenerateJwtToken(user);
        return new AuthView { Token = token, Succeeded = true, RequiresMfa = false };
    }

    public async ValueTask<AuthView> VerifyGoogleMfaAsync(MfaRequest request, CancellationToken cancellationToken = default)
    {
        var settings = new GoogleJsonWebSignature.ValidationSettings
        {
            Audience = new[] { configuration["Google:ClientId"] }
        };
        var payload = await GoogleJsonWebSignature.ValidateAsync(request.Token, settings);
        var user = await userManager.FindByEmailAsync(payload.Email);
        if (user == null)
        {
            return new AuthView { Message = "User not found", Succeeded = false };
        }
        var token = GenerateJwtToken(user);
        return new AuthView { Token = token, Succeeded = true, RequiresMfa = false };
    }


    private string GenerateJwtToken(IdentityUser user)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email!),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"],
            audience: configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddHours(1),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
