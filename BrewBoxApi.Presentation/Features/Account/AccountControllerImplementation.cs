using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BrewBoxApi.Presentation.Features.Account.MfaCommand;
using BrewBoxApi.Presentation.Features.Account.RegisterCommand;
using BrewBoxApi.Presentation.Features.Auth.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Google.Apis.Auth;
using FluentValidation;
using BrewBoxApi.Presentation.Features.SeedWork;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace BrewBoxApi.Presentation.Features.Account;

internal sealed class AccountControllerImplementation(
UserManager<IdentityUser> userManager,
RoleManager<IdentityRole> roleManager,
IConfiguration configuration
) : IAccountControllerImplementation
{

    public async ValueTask<BaseResponse<string>> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var validator = new RegisterValidator();
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        var user = new IdentityUser { UserName = request.Email, Email = request.Email };
        var result = await userManager.CreateAsync(user, request.Password!);

        if (!result.Succeeded)
        {
            return BaseResponse<string>.Failed(result.Errors.Select(x => x.Description));
        }

        if (!await roleManager.RoleExistsAsync(request.Role!))
        {
            await roleManager.CreateAsync(new IdentityRole(request.Role!));
        }

        await userManager.AddToRoleAsync(user, request.Role!);
        return BaseResponse<string>.Succeeded(user.Id);
    }

    public async ValueTask<BaseResponse<AuthView>> VerifyAppleMfaAsync(MfaRequest request, CancellationToken cancellationToken = default)
    {  // Mock Apple Sign-In validation (requires Apple Developer setup in production)
        if (string.IsNullOrEmpty(request.Token) || !request.Token.StartsWith("mock_apple_"))
        {

            return BaseResponse<AuthView>.Failed(["Invalid Apple token"]);
        }
        var email = request.Token.Replace("mock_apple_", "");
        var user = await userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return BaseResponse<AuthView>.Failed(["User not found."]);
        }
        var token = GenerateJwtToken(user);
        var result = new AuthView { Token = token, RequiresMfa = false };
        return BaseResponse<AuthView>.Succeeded(result);
    }

    public async ValueTask<BaseResponse<AuthView>> VerifyGoogleMfaAsync(MfaRequest request, CancellationToken cancellationToken = default)
    {
        var settings = new GoogleJsonWebSignature.ValidationSettings
        {
            Audience = new[] { configuration["Google:ClientId"] }
        };
        var payload = await GoogleJsonWebSignature.ValidateAsync(request.Token, settings);
        var user = await userManager.FindByEmailAsync(payload.Email);
        if (user == null)
        {
            return BaseResponse<AuthView>.Failed(["User not found."]);
        }
        var token = GenerateJwtToken(user);
        var result = new AuthView { Token = token, RequiresMfa = false };
        return BaseResponse<AuthView>.Succeeded(result);
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
