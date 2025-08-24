using Microsoft.AspNetCore.Identity;

namespace BrewBoxApi.Application.CQRS.Auth.Models;

public sealed record AuthView
{
    public string? Token { get; set; }
    public string? RefreshToken { get; set; }
    public string[]? Roles { get; set; }

    internal static Func<string, IList<string>, AuthView> MapFrom => (token, roles) =>
    {
        return new AuthView
        {
            RefreshToken = string.Empty,
            Token = token,
            Roles = [.. roles]
        };
    };
}