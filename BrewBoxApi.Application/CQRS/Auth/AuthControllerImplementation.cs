using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BrewBoxApi.Application.Common.SeedWork;
using BrewBoxApi.Application.CQRS.Auth.LoginCommand;
using BrewBoxApi.Application.CQRS.Auth.Models;
using BrewBoxApi.Domain.Aggregates.Identity;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace BrewBoxApi.Application.CQRS.Auth;

public sealed class AuthControllerImplementation(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    IConfiguration configuration) : IAuthControllerImplementation
{
    public async ValueTask<BaseResponse<AuthView>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var validator = new LoginValidator();
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        var user = await userManager.FindByEmailAsync(request.Email!);

        if (user == null)
        {
            return BaseResponse<AuthView>.Failed(["User could not be authenticated."]);
        }

        var identityResult = await signInManager.CheckPasswordSignInAsync(user, request.Password!, false);
        if (!identityResult.Succeeded)
        {
            return BaseResponse<AuthView>.Failed(["User could not be authenticated."]);
        }

        var token = await GenerateJwtTokenAsync(user, cancellationToken);
        var roles = await userManager.GetRolesAsync(user);

        var result = new AuthView
        {
            Token = token,
            Roles = [.. roles],
            RefreshToken = ""
        };

        return BaseResponse<AuthView>.Succeeded(result);
    }

    public async ValueTask<BaseResponse<AuthView>> ExternalLoginAsync(ClaimsPrincipal claims, CancellationToken cancellationToken = default)
    {
        var email = claims.FindFirst(ClaimTypes.Email)?.Value;
        var name = claims.FindFirst(ClaimTypes.Name)?.Value;
        if (string.IsNullOrEmpty(email))
        {
            return BaseResponse<AuthView>.Failed(["Email not provided by external provider."]);
        }

        var user = await userManager.FindByEmailAsync(email);
        if (user == null)
        {
            user = new ApplicationUser { UserName = email, Email = email, DisplayName = name ?? "System" };
            var identityResult = await userManager.CreateAsync(user);
            if (!identityResult.Succeeded)
            {
                return BaseResponse<AuthView>.Failed(identityResult.Errors.Select(e => e.Description));
            }
        }

        var token = await GenerateJwtTokenAsync(user, cancellationToken);

        var result = new AuthView { Token = token, RequiresMfa = false };
        return BaseResponse<AuthView>.Succeeded(result);
    }


    private async Task<string> GenerateJwtTokenAsync(ApplicationUser user, CancellationToken cancellationToken = default)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email!),
            new Claim(ClaimTypes.NameIdentifier, user.Id)
        };

        var roles = await userManager.GetRolesAsync(user);
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var jwtSettings = configuration.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}