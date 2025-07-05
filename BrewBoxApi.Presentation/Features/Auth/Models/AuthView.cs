namespace BrewBoxApi.Presentation.Features.Auth.Models;

public sealed record AuthView
{
    public string? Token { get; set; }
    public string? RefreshToken { get; set; }
    public string? Message { get; set; }
    public bool Succeeded { get; set; }
    public List<string>? Details { get; set; }
}