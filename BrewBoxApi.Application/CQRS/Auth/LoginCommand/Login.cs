using FluentValidation;

namespace BrewBoxApi.Application.CQRS.Auth.LoginCommand;
/// <summary>
/// Represents a request to log in with user credentials.
/// </summary>
/// <param name="Email">The email address of the user attempting to log in.</param>
/// <param name="Password">The password associated with the user's account.</param>
 public record LoginRequest(string? Email, string? Password);

internal sealed class LoginValidator : AbstractValidator<LoginRequest>
{
    public LoginValidator()
    {
        RuleFor(x => x.Email)
        .NotNull().WithMessage("Please provide an email address.")
        .EmailAddress().WithMessage("Please provide a valid email address.");

        RuleFor(x => x.Password)
        .NotEmpty().WithMessage("Password is required.")
        .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
        .Matches(@"\d").WithMessage("Password must contain at least one digit.");
    }
}