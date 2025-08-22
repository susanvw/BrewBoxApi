using BrewBoxApi.Application.Common.SeedWork;
using BrewBoxApi.Application.CQRS.Account.RegisterCommand;

namespace BrewBoxApi.Application.CQRS.Account;

public interface IAccountControllerImplementation
{
    ValueTask<BaseResponse<string>> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);
}
