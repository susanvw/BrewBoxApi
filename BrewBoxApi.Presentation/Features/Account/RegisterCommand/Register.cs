using BrewBoxApi.Domain.Aggregates.Identity;
using BrewBoxApi.Presentation.Features.SeedWork;
using FluentValidation;

namespace BrewBoxApi.Presentation.Features.Account.RegisterCommand;

/// <summary>
/// Represents a request to register a new user account.
/// </summary>
/// <remarks>
/// Contains the necessary information required for user registration, including email, password, and role.
/// </remarks>
public sealed record RegisterRequest
{
    public string? Email { get; init; }
    public string? Password { get; init; }
    public string? Role { get; init; }
}

internal sealed class RegisterValidator : AbstractValidator<RegisterRequest>
{
    public RegisterValidator()
    {
        RuleFor(x => x.Email)
        .NotNull().WithMessage("Please provide an email address.")
        .EmailAddress().WithMessage("Please provide a valid email address.");

        RuleFor(x => x.Password)
        .NotEmpty().WithMessage("Password is required.")
        .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
        .Matches(@"\d").WithMessage("Password must contain at least one digit.");

        RuleFor(x => x.Role)
        .NotEmpty().WithMessage("Please provide a role name.")
        .IsEnumName(typeof(RoleType), false).WithMessage("Please provide an existing role.");
    }
}
