using BrewBoxApi.Application.Common.SeedWork;
using BrewBoxApi.Application.CQRS.Account.RegisterCommand;
using BrewBoxApi.Domain.Aggregates.Identity;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace BrewBoxApi.Application.CQRS.Account;

public sealed class AccountControllerImplementation(
UserManager<ApplicationUser> userManager,
RoleManager<IdentityRole<string>> roleManager
) : IAccountControllerImplementation
{

    public async ValueTask<BaseResponse<string>> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var validator = new RegisterValidator();
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        var user = new ApplicationUser { UserName = request.Email, Email = request.Email, DisplayName = request.DisplayName! };
        var result = await userManager.CreateAsync(user, request.Password!);

        if (!result.Succeeded)
        {
            return BaseResponse<string>.Failed(result.Errors.Select(x => x.Description));
        }

        if (!await roleManager.RoleExistsAsync(request.Role!))
        {
            await roleManager.CreateAsync(new IdentityRole(request.Role!));
        }

        await userManager.AddToRoleAsync(user, request.Role!);
        return BaseResponse<string>.Succeeded(user.Id);
    }

}
