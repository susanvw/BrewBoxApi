using BrewBoxApi.Application.Common.SeedWork;
using BrewBoxApi.Application.CQRS.Auth.LoginCommand;
using BrewBoxApi.Application.CQRS.Auth.Models;

namespace BrewBoxApi.Application.CQRS.Auth;

public interface IAuthControllerImplementation
{
    ValueTask<BaseResponse<AuthView>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
}