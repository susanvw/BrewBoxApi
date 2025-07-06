using BrewBoxApi.Presentation.Features.Account.MfaCommand;
using BrewBoxApi.Presentation.Features.Account.RegisterCommand;
using BrewBoxApi.Presentation.Features.Auth.Models;
using Microsoft.AspNetCore.Identity;

namespace BrewBoxApi.Presentation.Features.Account;

public interface IAccountControllerImplementation
{
    ValueTask<IdentityResult> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);
    ValueTask<AuthView> VerifyGoogleMfaAsync(MfaRequest request, CancellationToken cancellationToken = default);
    ValueTask<AuthView> VerifyAppleMfaAsync(MfaRequest request, CancellationToken cancellationToken = default);
}
