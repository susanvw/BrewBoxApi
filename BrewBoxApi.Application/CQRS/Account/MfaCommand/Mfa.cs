namespace BrewBoxApi.Application.CQRS.Account.MfaCommand;

public class MfaRequest
{
    public string Token { get; init; } = string.Empty;
}