using BrewBoxApi.Presentation.Features.Account.MfaCommand;
using BrewBoxApi.Presentation.Features.Account.RegisterCommand;
using BrewBoxApi.Presentation.Features.Auth.Models;
using BrewBoxApi.Presentation.Features.SeedWork;

namespace BrewBoxApi.Presentation.Features.Account;

public interface IAccountControllerImplementation
{
    ValueTask<BaseResponse<string>> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);
    ValueTask<BaseResponse<AuthView>> VerifyGoogleMfaAsync(MfaRequest request, CancellationToken cancellationToken = default);
    ValueTask<BaseResponse<AuthView>> VerifyAppleMfaAsync(MfaRequest request, CancellationToken cancellationToken = default);
}
