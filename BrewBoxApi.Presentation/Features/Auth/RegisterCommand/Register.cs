namespace BrewBoxApi.Presentation.Features.Auth.RegisterCommand;

public record RegisterRequest(string Email, string Password, bool IsBarista);