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

        return BaseResponse<AuthView>.Succeeded(AuthView.MapFrom(token, roles));
    }

    private async Task<string> GenerateJwtTokenAsync(ApplicationUser user, CancellationToken cancellationToken = default)
    {
        var jwtSettings = configuration.GetSection("Jwt");
        ArgumentException.ThrowIfNullOrEmpty(jwtSettings["Key"]);

        var jwtKey = jwtSettings["Key"]!;
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.Email, user.Email!),
            new(ClaimTypes.NameIdentifier, user.Id)
        };

        var roles = await userManager.GetRolesAsync(user);
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        return new JwtSecurityTokenHandler().WriteToken(new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)), SecurityAlgorithms.HmacSha256)));
    }
}