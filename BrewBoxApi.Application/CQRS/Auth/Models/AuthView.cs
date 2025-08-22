namespace BrewBoxApi.Application.CQRS.Auth.Models;

public sealed record AuthView
{
    public string? Token { get; set; }
    public string? RefreshToken { get; set; }
    public bool RequiresMfa { get; set; }
    public string[]? Roles { get; set; }
}