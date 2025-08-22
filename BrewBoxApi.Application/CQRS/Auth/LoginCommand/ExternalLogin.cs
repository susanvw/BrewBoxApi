namespace BrewBoxApi.Application.CQRS.Auth.LoginCommand;

public record ExternalLoginRequest(string Provider, string Token);