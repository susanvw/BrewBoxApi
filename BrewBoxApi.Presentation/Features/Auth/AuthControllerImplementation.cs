using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BrewBoxApi.Presentation.Features.Auth.LoginCommand;
using BrewBoxApi.Presentation.Features.Auth.Models;
using BrewBoxApi.Presentation.Features.Auth.RegisterCommand;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace BrewBoxApi.Presentation.Features.Auth;

internal sealed class AuthControllerImplementation(
    UserManager<IdentityUser> userManager,
    SignInManager<IdentityUser> signInManager,
    IConfiguration configuration) : IAuthControllerImplementation
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

    public async ValueTask<AuthView> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        
        if (user == null)
        {
            return new AuthView
            {
                Succeeded = false,
                Message = "Invalid email or password"
            };
        }

        var result = await signInManager.CheckPasswordSignInAsync(user, request.Password, false);
        if (!result.Succeeded)
        {
            return new AuthView
            {
                Succeeded = false,
                Message = "Invalid email or password"
            };
        }

        var token = await GenerateJwtTokenAsync(user, cancellationToken);

        return new AuthView
        {
            Succeeded = true,
            Token = token,
            RefreshToken = ""
        };
    }

    public async Task<AuthView> ExternalLoginAsync(ClaimsPrincipal claims, CancellationToken cancellationToken = default)
    {
        var email = claims.FindFirst(ClaimTypes.Email)?.Value;
        if (string.IsNullOrEmpty(email))
        {
            return new AuthView
            {
                Message = "Email not provided by external provider",
                Succeeded = false
            };
        }

        var user = await userManager.FindByEmailAsync(email);
        if (user == null)
        {
            user = new IdentityUser { UserName = email, Email = email };
            var result = await userManager.CreateAsync(user);
            if (!result.Succeeded)
            {
                return new AuthView
                {
                    Succeeded = false,
                    Message = "Failed to create user",
                    Details = result.Errors.Select(e => e.Description).ToList()
                };
            }
        }

        var token = await GenerateJwtTokenAsync(user, cancellationToken);

        return new AuthView
        {
            Succeeded = true,
            Token = token,
            RefreshToken = ""
        };
    }


    private async Task<string> GenerateJwtTokenAsync(IdentityUser user, CancellationToken cancellationToken = default)
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