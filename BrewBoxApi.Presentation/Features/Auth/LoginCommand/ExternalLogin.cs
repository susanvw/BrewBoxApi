namespace BrewBoxApi.Presentation.Features.Auth.LoginCommand;

public record ExternalLoginRequest(string Provider, string Token);